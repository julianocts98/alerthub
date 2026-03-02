using AlertHub.Domain.Common;

namespace AlertHub.Application.Deliveries;

public interface IDeliveryService
{
    Task<IReadOnlyCollection<DeliveryListItem>> GetDeliveriesAsync(GetDeliveriesRequest request, CancellationToken ct);
    Task<Result> RetryDeliveryAsync(Guid id, CancellationToken ct);
}
