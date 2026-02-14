using AlertHub.Api.Alerts;
using AlertHub.Application.Alerts.Ingestion;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Controllers;

[ApiController]
[Route("api/alerts")]
[Produces("application/json")]
public sealed class AlertsIngestionController : ControllerBase
{
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
    public async Task<IActionResult> Ingest([FromBody] AlertIngestionRequest request, CancellationToken ct)
    {
        var result = await _service.ExecuteAsync(request, ct);
        if (!result.IsSuccess || result.Value is null)
            return IngestionProblemDetailsMapper.ToActionResult(result.Error);

        return CreatedAtAction(
            nameof(Ingest),
            routeValues: null,
            value: result.Value);
    }
}
