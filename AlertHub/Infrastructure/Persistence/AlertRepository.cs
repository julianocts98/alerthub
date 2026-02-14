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
        var entity = new IngestedAlertEntity
        {
            Id = Guid.NewGuid(),
            Identifier = alert.Identifier,
            Sender = alert.Sender,
            Sent = alert.Sent,
            Status = alert.Status,
            MessageType = alert.MessageType,
            Scope = alert.Scope,
            RawPayload = rawPayload,
            ContentType = contentType,
            IngestedAtUtc = now
        };

        _dbContext.IngestedAlerts.Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        return new AlertPersistenceResult(entity.Id, entity.IngestedAtUtc);
    }
}
