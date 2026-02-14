using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Infrastructure.Persistence.Entities;
using DomainAlert = AlertHub.Domain.Alert.Alert;

namespace AlertHub.Infrastructure.Persistence;

public sealed class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _dbContext;

    public AlertRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AlertPersistenceResult> AddAsync(
        DomainAlert alert,
        string rawPayload,
        string contentType,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var id = Guid.NewGuid();

        var entity = new AlertEntity
        {
            Id = id,
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
        };

        _dbContext.Alerts.Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        return new AlertPersistenceResult(entity.Id, entity.IngestedAtUtc);
    }
}
