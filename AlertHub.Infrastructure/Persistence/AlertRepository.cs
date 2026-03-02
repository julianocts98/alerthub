using System.Text;
using System.Text.Json;
using AlertHub.Application.Alerts;
using AlertHub.Application.Alerts.Query;
using AlertHub.Domain.Alert;
using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using DomainAlert = AlertHub.Domain.Alert.Alert;

namespace AlertHub.Infrastructure.Persistence;

public sealed class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _dbContext;

    public AlertRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(string sender, string identifier, CancellationToken ct)
    {
        return await _dbContext.Alerts.AnyAsync(a => a.Sender == sender && a.Identifier == identifier, ct);
    }

    public Task<AlertPersistenceResult> AddAsync(
        DomainAlert alert,
        string rawPayload,
        string contentType,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var entity = new AlertEntity
        {
            Id = alert.Id,
            Identifier = alert.Identifier,
            Sender = alert.Sender,
            Sent = alert.Sent,
            Status = alert.Status,
            MessageType = alert.MessageType,
            Scope = alert.Scope,
            Source = alert.Source,
            Restriction = alert.Restriction,
            Note = alert.Note,
            Addresses = alert.Addresses.Count > 0 ? string.Join(" ", alert.Addresses) : null,
            Codes = alert.Codes.Count > 0 ? string.Join(" ", alert.Codes) : null,
            References = alert.References.Count > 0 ? string.Join(" ", alert.References.Select(r => r.ToString())) : null,
            Incidents = alert.Incidents.Count > 0 ? string.Join(" ", alert.Incidents) : null,
            RawPayload = rawPayload,
            ContentType = contentType,
            IngestedAtUtc = now,
            Infos = alert.Infos.Select(i => MapInfo(alert.Id, i)).ToList(),
        };

        foreach (var @event in alert.DomainEvents)
        {
            entity.AddDomainEvent(@event);
        }

        _dbContext.Alerts.Add(entity);

        return Task.FromResult(new AlertPersistenceResult(entity.Id, entity.IngestedAtUtc));
    }

    public async Task<AlertPage> SearchAsync(AlertSearchQuery query, CancellationToken ct)
    {
        var cursor = DecodeCursor(query.Cursor);

        var q = _dbContext.Alerts
            .AsNoTracking()
            .AsSplitQuery()
            .Include(a => a.Infos)
                .ThenInclude(i => i.Categories)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Sender))
            q = q.Where(a => a.Sender == query.Sender);

        if (!string.IsNullOrWhiteSpace(query.Identifier))
            q = q.Where(a => a.Identifier == query.Identifier);

        if (query.SentFrom.HasValue)
            q = q.Where(a => a.Sent >= query.SentFrom.Value);

        if (query.SentTo.HasValue)
            q = q.Where(a => a.Sent <= query.SentTo.Value);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<AlertStatus>(query.Status, out var status))
            q = q.Where(a => a.Status == status);

        if (!string.IsNullOrWhiteSpace(query.MessageType) && Enum.TryParse<AlertMessageType>(query.MessageType, out var msgType))
            q = q.Where(a => a.MessageType == msgType);

        if (!string.IsNullOrWhiteSpace(query.Scope) && Enum.TryParse<AlertScope>(query.Scope, out var scope))
            q = q.Where(a => a.Scope == scope);

        if (!string.IsNullOrWhiteSpace(query.Event))
            q = q.Where(a => a.Infos.Any(i => i.Event == query.Event));

        if (!string.IsNullOrWhiteSpace(query.Urgency) && Enum.TryParse<AlertUrgency>(query.Urgency, out var urgency))
            q = q.Where(a => a.Infos.Any(i => i.Urgency == urgency));

        if (!string.IsNullOrWhiteSpace(query.Severity) && Enum.TryParse<AlertSeverity>(query.Severity, out var severity))
            q = q.Where(a => a.Infos.Any(i => i.Severity == severity));

        if (!string.IsNullOrWhiteSpace(query.Certainty) && Enum.TryParse<AlertCertainty>(query.Certainty, out var certainty))
            q = q.Where(a => a.Infos.Any(i => i.Certainty == certainty));

        if (!string.IsNullOrWhiteSpace(query.Category) && Enum.TryParse<AlertInfoCategory>(query.Category, out var category))
            q = q.Where(a => a.Infos.Any(i => i.Categories.Any(c => c.Category == category)));

        // Keyset boundary: exclude everything at or before the last seen (sent, ingested_at_utc) pair.
        // ORDER BY sent DESC, ingested_at_utc DESC — so "after cursor" means an earlier sent,
        // or the same sent but ingested earlier.
        if (cursor is not null)
        {
            var (cursorSent, cursorIngestedAt) = cursor.Value;
            q = q.Where(a =>
                a.Sent < cursorSent ||
                (a.Sent == cursorSent && a.IngestedAtUtc < cursorIngestedAt));
        }

        var items = await q
            .OrderByDescending(a => a.Sent)
            .ThenByDescending(a => a.IngestedAtUtc)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var nextCursor = items.Count == query.PageSize
            ? EncodeCursor(items[^1].Sent, items[^1].IngestedAtUtc)
            : null;

        return new AlertPage(items.Select(ToQueryResult).ToList(), nextCursor);
    }

    private static string EncodeCursor(DateTimeOffset sent, DateTimeOffset ingestedAt)
    {
        var json = JsonSerializer.Serialize(new { sent, ingestedAt });
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    private static (DateTimeOffset Sent, DateTimeOffset IngestedAt)? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;

        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            using var doc = JsonDocument.Parse(json);
            var sent = doc.RootElement.GetProperty("sent").GetDateTimeOffset();
            var ingestedAt = doc.RootElement.GetProperty("ingestedAt").GetDateTimeOffset();
            return (sent, ingestedAt);
        }
        catch
        {
            return null;
        }
    }

    private static AlertQueryResult ToQueryResult(AlertEntity a) => new()
    {
        Id = a.Id,
        Identifier = a.Identifier,
        Sender = a.Sender,
        Sent = a.Sent,
        Status = a.Status.ToString(),
        MessageType = a.MessageType.ToString(),
        Scope = a.Scope.ToString(),
        IngestedAtUtc = a.IngestedAtUtc,
        Infos = a.Infos.Select(i => new AlertInfoQueryResult
        {
            Id = i.Id,
            Event = i.Event,
            Urgency = i.Urgency.ToString(),
            Severity = i.Severity.ToString(),
            Certainty = i.Certainty.ToString(),
            Categories = i.Categories.Select(c => c.Category.ToString()).ToList(),
            Effective = i.Effective,
            Onset = i.Onset,
            Expires = i.Expires,
            Headline = i.Headline,
        }).ToList(),
    };

    private static AlertInfoEntity MapInfo(Guid alertId, AlertInfo info)
    {
        var infoEntity = new AlertInfoEntity
        {
            Id = info.Id,
            AlertId = alertId,
            Event = info.Event,
            Urgency = info.Urgency,
            Severity = info.Severity,
            Certainty = info.Certainty,
            Language = info.Language,
            Audience = info.Audience,
            Effective = info.Effective,
            Onset = info.Onset,
            Expires = info.Expires,
            SenderName = info.SenderName,
            Headline = info.Headline,
            Description = info.Description,
            Instruction = info.Instruction,
            Web = info.Web,
            Contact = info.Contact,
            Categories = info.Categories
                .Select(c => new AlertInfoCategoryEntity { AlertInfoId = info.Id, Category = c })
                .ToList(),
            ResponseTypes = info.ResponseTypes
                .Select(r => new AlertInfoResponseTypeEntity { AlertInfoId = info.Id, ResponseType = r })
                .ToList(),
            EventCodes = info.EventCodes
                .Select(e => new AlertInfoEventCodeEntity { Id = Guid.NewGuid(), AlertInfoId = info.Id, ValueName = e.ValueName, Value = e.Value })
                .ToList(),
            Parameters = info.Parameters
                .Select(p => new AlertInfoParameterEntity { Id = Guid.NewGuid(), AlertInfoId = info.Id, ValueName = p.ValueName, Value = p.Value })
                .ToList(),
            Resources = info.Resources
                .Select(r => new AlertInfoResourceEntity
                {
                    Id = Guid.NewGuid(),
                    AlertInfoId = info.Id,
                    ResourceDescription = r.ResourceDescription,
                    MimeType = r.MimeType,
                    Size = r.Size,
                    Uri = r.Uri,
                    Digest = r.Digest,
                })
                .ToList(),
            Areas = info.Areas.Select(MapArea).ToList(),
        };

        return infoEntity;
    }

    private static AlertAreaEntity MapArea(AlertArea area) => new()
    {
        Id = area.Id,
        AreaDescription = area.AreaDescription,
        Altitude = area.Altitude,
        Ceiling = area.Ceiling,
        Polygons = area.Polygons
            .Select(p => new AlertAreaPolygonEntity { Id = Guid.NewGuid(), AlertAreaId = area.Id, Points = p.ToString() })
            .ToList(),
        Circles = area.Circles
            .Select(c => new AlertAreaCircleEntity
            {
                Id = Guid.NewGuid(),
                AlertAreaId = area.Id,
                CenterLatitude = c.Center.Latitude,
                CenterLongitude = c.Center.Longitude,
                Radius = c.Radius,
            })
            .ToList(),
        GeoCodes = area.GeoCodes
            .Select(g => new AlertAreaGeoCodeEntity { Id = Guid.NewGuid(), AlertAreaId = area.Id, ValueName = g.ValueName, Value = g.Value })
            .ToList(),
    };
}
