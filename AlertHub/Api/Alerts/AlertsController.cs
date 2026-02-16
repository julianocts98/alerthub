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
        [FromQuery] string? sender,
        [FromQuery] string? identifier,
        [FromQuery] DateTimeOffset? sentFrom,
        [FromQuery] DateTimeOffset? sentTo,
        [FromQuery] string? status,
        [FromQuery] string? messageType,
        [FromQuery] string? scope,
        [FromQuery] string? @event,
        [FromQuery] string? urgency,
        [FromQuery] string? severity,
        [FromQuery] string? certainty,
        [FromQuery] string? category,
        [FromQuery] string? cursor = null,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var query = new AlertSearchQuery
        {
            Sender = sender,
            Identifier = identifier,
            SentFrom = sentFrom,
            SentTo = sentTo,
            Status = status,
            MessageType = messageType,
            Scope = scope,
            Event = @event,
            Urgency = urgency,
            Severity = severity,
            Certainty = certainty,
            Category = category,
            Cursor = cursor,
            PageSize = pageSize,
        };

        var page = await _queryService.SearchAsync(query, ct);
        return Ok(page);
    }
}
