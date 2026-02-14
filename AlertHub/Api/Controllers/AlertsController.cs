using AlertHub.Api.Alerts;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Alerts.Query;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Controllers;

[ApiController]
[Route("api/alerts")]
[Produces("application/json")]
public sealed class AlertsController : ControllerBase
{
    private readonly IngestAlertOrchestrationService _ingestService;
    private readonly IAlertRepository _alertRepository;

    public AlertsController(IngestAlertOrchestrationService ingestService, IAlertRepository alertRepository)
    {
        _ingestService = ingestService;
        _alertRepository = alertRepository;
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

        return CreatedAtAction(nameof(Ingest), routeValues: null, value: result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertQueryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AlertQueryResult>>> Search(
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
        [FromQuery] int page = 1,
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
            Page = page,
            PageSize = pageSize,
        };

        var results = await _alertRepository.SearchAsync(query, ct);
        return Ok(results);
    }
}
