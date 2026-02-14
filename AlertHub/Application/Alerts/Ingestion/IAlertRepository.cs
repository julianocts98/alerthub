using AlertHub.Application.Alerts.Query;
using DomainAlert = AlertHub.Domain.Alert.Alert;

namespace AlertHub.Application.Alerts.Ingestion;

public interface IAlertRepository
{
    Task<AlertPersistenceResult> AddAsync(
        DomainAlert alert,
        string rawPayload,
        string contentType,
        CancellationToken ct);

    Task<IReadOnlyList<AlertQueryResult>> SearchAsync(
        AlertSearchQuery query,
        CancellationToken ct);
}

public sealed record AlertPersistenceResult(Guid Id, DateTimeOffset IngestedAtUtc);
