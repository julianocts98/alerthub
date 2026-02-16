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
        return (await _deliveryService.RetryDeliveryAsync(id, ct)).ToActionResult();
    }
}
