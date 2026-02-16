using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Entities.Deliveries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DeliveriesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public DeliveriesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetDeliveries([FromQuery] DeliveryStatus? status, [FromQuery] int limit = 50)
    {
        var query = _dbContext.AlertDeliveries.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        var deliveries = await query
            .OrderByDescending(d => d.Id) // Assuming newer IDs are generally later
            .Take(limit)
            .ToListAsync();

        return Ok(deliveries);
    }

    [HttpPost("{id:guid}/retry")]
    public async Task<IActionResult> RetryDelivery(Guid id)
    {
        var delivery = await _dbContext.AlertDeliveries.FindAsync(id);

        if (delivery == null)
        {
            return NotFound();
        }

        if (delivery.Status != DeliveryStatus.Failed)
        {
            return BadRequest("Only failed deliveries can be retried.");
        }

        delivery.Status = DeliveryStatus.Pending;
        delivery.RetryCount = 0;
        delivery.Error = null;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
