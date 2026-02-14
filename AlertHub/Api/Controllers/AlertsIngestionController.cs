using AlertHub.Api.Alerts;
using AlertHub.Application.Alerts.Ingestion;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Controllers;

[ApiController]
[Route("api/alerts")]
[Produces("application/json")]
public sealed class AlertsIngestionController : ControllerBase
{
    private readonly IngestAlertOrchestrationService _ingestService;

    public AlertsIngestionController(IngestAlertOrchestrationService ingestService)
    {
        _ingestService = ingestService;
    }

    [HttpPost("ingest")]
    [Consumes("application/json", "application/xml")]
    [ProducesResponseType(typeof(AlertIngestionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Ingest(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var rawPayload = await reader.ReadToEndAsync(ct);

        var contentType = Request.ContentType ?? string.Empty;
        var result = await _ingestService.ExecuteAsync(rawPayload, contentType, ct);
        if (!result.IsSuccess || result.Value is null)
            return IngestionProblemDetailsMapper.ToActionResult(result.Error);

        return CreatedAtAction(
            nameof(Ingest),
            routeValues: null,
            value: result.Value);
    }
}
