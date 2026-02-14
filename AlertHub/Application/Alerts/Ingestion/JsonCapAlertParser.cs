using System.Text.Json;
using System.Text.Json.Serialization;
using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Ingestion;

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
            var request = JsonSerializer.Deserialize<AlertIngestionRequest>(rawPayload, JsonOptions);
            if (request is null)
            {
                return Result<AlertIngestionRequest>.Failure(
                    new ResultError(IngestionErrorCodes.InvalidPayload, "Payload could not be parsed into a CAP alert request."));
            }

            return Result<AlertIngestionRequest>.Success(request);
        }
        catch (JsonException ex)
        {
            return Result<AlertIngestionRequest>.Failure(
                new ResultError(IngestionErrorCodes.InvalidPayload, $"JSON payload is invalid: {ex.Message}"));
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
