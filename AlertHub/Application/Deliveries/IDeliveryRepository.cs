namespace AlertHub.Application.Deliveries;

public interface IDeliveryRepository
{
    Task<IReadOnlyCollection<DeliveryListItem>> GetAsync(DeliveryStatusFilter? status, int limit, CancellationToken ct);
    Task<RetryDeliveryResult> RetryFailedAsync(Guid id, CancellationToken ct);
}
