using System.Text.Json;
using System.Text.Json.Serialization;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Common;
using AlertHub.Infrastructure.Alerts.Ingestion.Transport;

namespace AlertHub.Infrastructure.Alerts.Ingestion;

public sealed class JsonCapAlertParser : ICapAlertParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public bool CanHandle(string contentType) => Matches(contentType, "application/json") || Matches(contentType, "text/json");

    public Result<AlertIngestionRequest> Parse(string rawPayload)
    {
        try
        {
            var transportRequest = JsonSerializer.Deserialize<CapAlertTransportRequest>(rawPayload, JsonOptions);
            if (transportRequest is null)
            {
                return Result<AlertIngestionRequest>.Failure(
                    ResultError.BadRequest(IngestionErrorCodes.InvalidPayload, "Payload could not be parsed into a CAP alert request."));
            }

            var request = CapAlertTransportMapper.ToApplicationRequest(transportRequest);
            return Result<AlertIngestionRequest>.Success(request);
        }
        catch (JsonException ex)
        {
            return Result<AlertIngestionRequest>.Failure(
                ResultError.BadRequest(IngestionErrorCodes.InvalidPayload, $"JSON payload is invalid: {ex.Message}"));
        }
    }

    private static bool Matches(string contentType, string mediaType)
    {
        var normalized = Normalize(contentType);
        return string.Equals(normalized, mediaType, StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return string.Empty;

        var parts = contentType.Split(';', 2, StringSplitOptions.TrimEntries);
        return parts[0];
    }
}
