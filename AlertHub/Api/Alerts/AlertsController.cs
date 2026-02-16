using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Alerts.Query;
using AlertHub.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Alerts;

[ApiController]
[Route("api/alerts")]
[Produces("application/json")]
[Authorize]
public sealed class AlertsController : ControllerBase
{
    private readonly AlertIngestionService _ingestService;
    private readonly AlertQueryService _queryService;

    public AlertsController(AlertIngestionService ingestService, AlertQueryService queryService)
    {
        _ingestService = ingestService;
        _queryService = queryService;
    }

    [HttpPost("ingest")]
    [Authorize(Policy = Scopes.AlertsIngest)]
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

        if (result.Value.WasAlreadyIngested)
            return Ok(result.Value);

        return CreatedAtAction(nameof(Ingest), routeValues: null, value: result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(AlertPage), StatusCodes.Status200OK)]
    public async Task<ActionResult<AlertPage>> Search(
        [FromQuery] AlertSearchQuery query,
        CancellationToken ct = default)
    {
        var page = await _queryService.SearchAsync(query, ct);
        return Ok(page);
    }
}
