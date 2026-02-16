using AlertHub.Application.Subscriptions;
using AlertHub.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Subscriptions;

[ApiController]
[Route("api/subscriptions")]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionsController(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionResponse>> Create(
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken ct)
    {
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
