using AlertHub.Application.Deliveries;
using AlertHub.Infrastructure.Persistence.Entities.Deliveries;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure.Persistence.Deliveries;

public sealed class DeliveryRepository : IDeliveryRepository
{
    private readonly AppDbContext _dbContext;

    public DeliveryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<DeliveryListItem>> GetAsync(DeliveryStatusFilter? status, int limit, CancellationToken ct)
    {
        var query = _dbContext.AlertDeliveries.AsNoTracking().AsQueryable();

        if (status.HasValue)
        {
            var mappedStatus = status.Value switch
            {
                DeliveryStatusFilter.Pending => DeliveryStatus.Pending,
                DeliveryStatusFilter.Sent => DeliveryStatus.Sent,
                DeliveryStatusFilter.Failed => DeliveryStatus.Failed,
                _ => DeliveryStatus.Pending
            };

            query = query.Where(d => d.Status == mappedStatus);
        }

        var deliveries = await query
            .OrderByDescending(d => EF.Property<DateTimeOffset>(d, "CreatedAt"))
            .Take(limit)
            .Select(d => new DeliveryListItem(
                d.Id,
                d.AlertId,
                d.SubscriptionId,
                d.Target,
                d.Channel,
                d.Status.ToString(),
                d.ExternalReference,
                d.Error,
                d.RetryCount,
                d.SentAtUtc,
                EF.Property<DateTimeOffset>(d, "CreatedAt")))
            .ToListAsync(ct);

        return deliveries;
    }

    public async Task<RetryDeliveryResult> RetryFailedAsync(Guid id, CancellationToken ct)
    {
        var delivery = await _dbContext.AlertDeliveries.FindAsync([id], cancellationToken: ct);
        if (delivery is null)
            return RetryDeliveryResult.NotFound;

        if (delivery.Status != DeliveryStatus.Failed)
            return RetryDeliveryResult.NotFailed;

        delivery.Status = DeliveryStatus.Pending;
        delivery.RetryCount = 0;
        delivery.Error = null;

        await _dbContext.SaveChangesAsync(ct);

        return RetryDeliveryResult.Retried;
    }
}
