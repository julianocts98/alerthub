using AlertHub.Application.Alerts;
using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class AlertIngestionService
{
    private readonly IReadOnlyCollection<ICapAlertParser> _parsers;
    private readonly ICapXmlSchemaValidator _xmlSchemaValidator;
    private readonly AlertFactory _alertFactory;
    private readonly IAlertRepository _alertRepository;

    public AlertIngestionService(
        IEnumerable<ICapAlertParser> parsers,
        ICapXmlSchemaValidator xmlSchemaValidator,
        AlertFactory alertFactory,
        IAlertRepository alertRepository)
    {
        _parsers = parsers.ToArray();
        _xmlSchemaValidator = xmlSchemaValidator;
        _alertFactory = alertFactory;
        _alertRepository = alertRepository;
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

        var domainResult = await _alertFactory.CreateAsync(parseResult.Value, ct);
        if (!domainResult.IsSuccess || domainResult.Value is null)
        {
            return Result<AlertIngestionResponse>.Failure(
                domainResult.Error ?? new ResultError(IngestionErrorCodes.InvalidPayload, "Alert could not be validated."));
        }

        var persisted = await _alertRepository.AddAsync(domainResult.Value, rawPayload, contentType, ct);
        var response = new AlertIngestionResponse
        {
            Id = persisted.Id,
            Identifier = domainResult.Value.Identifier,
            Sent = domainResult.Value.Sent,
            IngestedAtUtc = persisted.IngestedAtUtc
        };

        return Result<AlertIngestionResponse>.Success(response);
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
