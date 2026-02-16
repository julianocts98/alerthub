namespace AlertHub.Application.Deliveries;

public sealed class DeliveryService
{
    private const int MaxLimit = 200;
    private readonly IDeliveryRepository _repository;

    public DeliveryService(IDeliveryRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<DeliveryListItem>> GetDeliveriesAsync(DeliveryStatusFilter? status, int limit, CancellationToken ct)
    {
        var effectiveLimit = limit <= 0 ? 50 : Math.Min(limit, MaxLimit);
        return _repository.GetAsync(status, effectiveLimit, ct);
    }

    public Task<RetryDeliveryResult> RetryDeliveryAsync(Guid id, CancellationToken ct)
        => _repository.RetryFailedAsync(id, ct);
}
