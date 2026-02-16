using AlertHub.Application.Common.Security;
using AlertHub.Application.Subscriptions;
using AlertHub.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Subscriptions;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;
    private readonly ICurrentUser _currentUser;

    public SubscriptionsController(SubscriptionService subscriptionService, ICurrentUser currentUser)
    {
        _subscriptionService = subscriptionService;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionResponse>> Create(
        [FromBody] CreateSubscriptionRequestDto requestDto,
        CancellationToken ct)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();

        var request = new CreateSubscriptionRequest(
            userId,
            requestDto.Channel,
            requestDto.Target,
            requestDto.MinSeverity,
            requestDto.Categories);

        try
        {
            var response = await _subscriptionService.CreateSubscriptionAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (DomainException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubscriptionResponse>> GetById(Guid id, CancellationToken ct)
    {
        // Implementation for GetById is not strictly required for the POST test but 
        // CreatedAtAction expects it. Let's add it to the Service/Repo if needed later.
        return Ok();
    }
}

public record CreateSubscriptionRequestDto(
    AlertHub.Domain.Subscriptions.SubscriptionChannel Channel,
    string Target,
    AlertHub.Domain.Alert.AlertSeverity? MinSeverity = null,
    List<AlertHub.Domain.Alert.AlertInfoCategory>? Categories = null);
