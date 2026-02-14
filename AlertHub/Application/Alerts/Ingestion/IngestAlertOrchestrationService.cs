using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class IngestAlertOrchestrationService
{
    private readonly IReadOnlyCollection<ICapAlertParser> _parsers;
    private readonly ICapXmlSchemaValidator _xmlSchemaValidator;
    private readonly IngestAlertService _ingestAlertService;

    public IngestAlertOrchestrationService(
        IEnumerable<ICapAlertParser> parsers,
        ICapXmlSchemaValidator xmlSchemaValidator,
        IngestAlertService ingestAlertService)
    {
        _parsers = parsers.ToArray();
        _xmlSchemaValidator = xmlSchemaValidator;
        _ingestAlertService = ingestAlertService;
    }

    public async Task<Result<AlertIngestionResponse>> ExecuteAsync(string rawPayload, string contentType, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var parser = _parsers.FirstOrDefault(p => p.CanHandle(contentType));
        if (parser is null)
        {
            return Result<AlertIngestionResponse>.Failure(
                new ResultError(IngestionErrorCodes.UnsupportedContentType, $"Unsupported media type '{contentType}'."));
        }

        if (IsXml(contentType))
        {
            var schemaValidation = _xmlSchemaValidator.Validate(rawPayload);
            if (!schemaValidation.IsSuccess)
                return Result<AlertIngestionResponse>.Failure(
                    schemaValidation.Error ?? new ResultError(IngestionErrorCodes.XmlSchemaInvalid, "XML schema validation failed."));
        }

        var parseResult = parser.Parse(rawPayload);
        if (!parseResult.IsSuccess || parseResult.Value is null)
        {
            return Result<AlertIngestionResponse>.Failure(
                parseResult.Error ?? new ResultError(IngestionErrorCodes.InvalidPayload, "Payload could not be parsed."));
        }

        return await _ingestAlertService.ExecuteAsync(parseResult.Value, ct);
    }

    private static bool IsXml(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        var mediaType = contentType.Split(';', 2, StringSplitOptions.TrimEntries)[0];
        return string.Equals(mediaType, "application/xml", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(mediaType, "text/xml", StringComparison.OrdinalIgnoreCase);
    }
}
