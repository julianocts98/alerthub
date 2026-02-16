using AlertHub.Api.Common;
using AlertHub.Application.Common.Security;
using AlertHub.Application.Deliveries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Deliveries;

[ApiController]
[Route("api/deliveries")]
[Authorize(Roles = Roles.Admin)]
public sealed class DeliveriesController : ControllerBase
{
    private readonly DeliveryService _deliveryService;

    public DeliveriesController(DeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDeliveries([FromQuery] GetDeliveriesRequest request, CancellationToken ct = default)
    {
        var deliveries = await _deliveryService.GetDeliveriesAsync(request, ct);
        return Ok(deliveries);
    }

    [HttpPost("{id:guid}/retry")]
    public async Task<IActionResult> RetryDelivery(Guid id, CancellationToken ct)
    {
        var result = await _deliveryService.RetryDeliveryAsync(id, ct);
        return result switch
        {
            RetryDeliveryResult.Retried => NoContent(),
            RetryDeliveryResult.NotFound => NotFound(),
            RetryDeliveryResult.NotFailed => ApiProblemDetails.Build(
                StatusCodes.Status400BadRequest,
                "Invalid delivery state",
                "Only failed deliveries can be retried."),
            _ => ApiProblemDetails.Build(
                StatusCodes.Status500InternalServerError,
                "Retry failed",
                "An unexpected delivery retry error occurred.")
        };
    }
}
