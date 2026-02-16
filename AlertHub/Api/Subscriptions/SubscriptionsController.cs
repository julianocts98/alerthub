using AlertHub.Api.Common;
using AlertHub.Application.Common.Security;
using AlertHub.Application.Subscriptions;
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
    public async Task<IActionResult> Create(
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiProblemDetails.Build(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "Authenticated user context is missing.");
        }

        var result = await _subscriptionService.CreateSubscriptionAsync(request, userId, ct);
        return result.ToActionResult(response =>
            CreatedAtAction(nameof(GetById), new { id = response!.Id }, response));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return (await _subscriptionService.GetByIdAsync(id, ct)).ToActionResult();
    }
}
