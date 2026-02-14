using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class IngestAlertService
{
    public Task<Result<AlertIngestionResponse>> ExecuteAsync(AlertIngestionRequest request, CancellationToken ct)
    {
        _ = ct;

        var now = DateTimeOffset.UtcNow;

        var response = new AlertIngestionResponse
        {
            Id = Guid.NewGuid(),
            Identifier = request.Identifier,
            Sent = request.Sent,
            IngestedAtUtc = now
        };

        return Task.FromResult(Result<AlertIngestionResponse>.Success(response));
    }
}
