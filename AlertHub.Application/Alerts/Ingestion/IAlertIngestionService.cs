using AlertHub.Domain.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public interface IAlertIngestionService
{
    Task<Result<AlertIngestionResponse>> ExecuteAsync(string rawPayload, string contentType, CancellationToken ct);
}
