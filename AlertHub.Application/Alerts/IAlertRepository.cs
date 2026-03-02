using AlertHub.Application.Alerts.Query;
using DomainAlert = AlertHub.Domain.Alert.Alert;

namespace AlertHub.Application.Alerts;

public interface IAlertRepository
{
    Task<bool> ExistsAsync(string sender, string identifier, CancellationToken ct);

    Task<AlertPersistenceResult> AddAsync(
        DomainAlert alert,
        string rawPayload,
        string contentType,
        CancellationToken ct);

    Task<AlertPage> SearchAsync(
        AlertSearchQuery query,
        CancellationToken ct);
}

public sealed record AlertPersistenceResult(Guid Id, DateTimeOffset IngestedAtUtc);
