using DomainAlert = AlertHub.Domain.Alert.Alert;

namespace AlertHub.Application.Alerts.Ingestion;

public interface IAlertRepository
{
    Task<AlertPersistenceResult> AddAsync(
        DomainAlert alert,
        string rawPayload,
        string contentType,
        CancellationToken ct);
}

public sealed record AlertPersistenceResult(Guid Id, DateTimeOffset IngestedAtUtc);
