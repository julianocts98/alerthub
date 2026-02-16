using System.Xml.Serialization;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Common;
using AlertHub.Infrastructure.Alerts.Ingestion.Transport;

namespace AlertHub.Infrastructure.Alerts.Ingestion;

public sealed class XmlCapAlertParser : ICapAlertParser
{
    private static readonly XmlSerializer Serializer = new(typeof(CapAlertTransportRequest));

    public bool CanHandle(string contentType) => Matches(contentType, "application/xml") || Matches(contentType, "text/xml");

    public Result<AlertIngestionRequest> Parse(string rawPayload)
    {
        try
        {
            using var reader = new StringReader(rawPayload);
            var transportRequest = Serializer.Deserialize(reader) as CapAlertTransportRequest;

            if (transportRequest is null)
            {
                return Result<AlertIngestionRequest>.Failure(
                    new ResultError(IngestionErrorCodes.InvalidPayload, "Payload could not be parsed into a CAP alert request."));
            }

            var request = CapAlertTransportMapper.ToApplicationRequest(transportRequest);
            return Result<AlertIngestionRequest>.Success(request);
        }
        catch (InvalidOperationException ex)
        {
            return Result<AlertIngestionRequest>.Failure(
                new ResultError(IngestionErrorCodes.InvalidPayload, $"XML payload is invalid: {ex.Message}"));
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
