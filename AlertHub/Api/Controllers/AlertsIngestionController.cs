using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Controllers;

[ApiController]
[Route("api/alerts")]
[Produces("application/json")]
public sealed class AlertsIngestionController : ControllerBase
{
    private static readonly HashSet<string> SupportedContentTypes =
    [
        "application/json",
        "application/xml"
    ];

    private readonly IngestAlertService _service;

    public AlertsIngestionController(IngestAlertService service)
    {
        _service = service;
    }

    [HttpPost("ingest")]
    [Consumes("application/json", "application/xml")]
    [ProducesResponseType(typeof(AlertIngestionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Ingest(CancellationToken ct)
    {
        var normalizedContentType = NormalizeMediaType(Request.ContentType);
        if (!SupportedContentTypes.Contains(normalizedContentType))
        {
            return Problem(
                title: "Unsupported content type",
                detail: "Only application/json and application/xml are supported.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        using var reader = new StreamReader(Request.Body);
        var rawPayload = await reader.ReadToEndAsync(ct);

        var result = await _service.ExecuteAsync(rawPayload, normalizedContentType, ct);
        if (!result.IsSuccess || result.Value is null)
        {
            return MapErrorToProblem(result.Error);
        }

        return CreatedAtAction(
            nameof(Ingest),
            routeValues: null,
            value: result.Value);
    }

    private IActionResult MapErrorToProblem(ResultError? error)
    {
        if (error is null)
        {
            return Problem(
                title: "Ingestion failed",
                detail: "Unexpected application error.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return error.Code switch
        {
            IngestionErrorCodes.UnsupportedContentType => Problem(
                title: "Unsupported content type",
                detail: error.Message,
                statusCode: StatusCodes.Status415UnsupportedMediaType),
            IngestionErrorCodes.EmptyPayload => Problem(
                title: "Invalid payload",
                detail: error.Message,
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(
                title: "Ingestion validation failed",
                detail: error.Message,
                statusCode: StatusCodes.Status422UnprocessableEntity)
        };
    }

    private static string NormalizeMediaType(string? contentTypeHeader)
    {
        if (string.IsNullOrWhiteSpace(contentTypeHeader))
            return string.Empty;

        var mediaType = contentTypeHeader.Split(';', 2)[0].Trim();
        return mediaType.ToLowerInvariant();
    }
}
