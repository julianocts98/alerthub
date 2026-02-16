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
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();

        try
        {
            var response = await _subscriptionService.CreateSubscriptionAsync(request, userId, ct);
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
        var response = await _subscriptionService.GetByIdAsync(id, ct);
        if (response is null)
            return NotFound();

        return Ok(response);
    }
}
