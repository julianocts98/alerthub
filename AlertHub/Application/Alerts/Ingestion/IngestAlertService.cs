using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class IngestAlertService
{
    public Task<Result<AlertIngestionResponse>> ExecuteAsync(string rawPayload, string contentType, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(rawPayload))
        {
            return Task.FromResult(
                Result<AlertIngestionResponse>.Failure(
                    new ResultError(IngestionErrorCodes.EmptyPayload, "Payload cannot be empty.")));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            return Task.FromResult(
                Result<AlertIngestionResponse>.Failure(
                    new ResultError(IngestionErrorCodes.UnsupportedContentType, "Content type is required.")));
        }

        var now = DateTimeOffset.UtcNow;

        var response = new AlertIngestionResponse
        {
            Id = Guid.NewGuid(),
            Identifier = "stub-identifier",
            Sent = now,
            IngestedAtUtc = now
        };

        return Task.FromResult(Result<AlertIngestionResponse>.Success(response));
    }
}
