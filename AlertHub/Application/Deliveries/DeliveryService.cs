namespace AlertHub.Application.Deliveries;

public sealed class DeliveryService
{
    private const int MaxLimit = 200;
    private readonly IDeliveryRepository _repository;

    public DeliveryService(IDeliveryRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<DeliveryListItem>> GetDeliveriesAsync(GetDeliveriesRequest request, CancellationToken ct)
    {
        var effectiveLimit = request.Limit <= 0 ? 50 : Math.Min(request.Limit, MaxLimit);
        return _repository.GetAsync(request.Status, effectiveLimit, ct);
    }

    public Task<RetryDeliveryResult> RetryDeliveryAsync(Guid id, CancellationToken ct)
        => _repository.RetryFailedAsync(id, ct);
}

public sealed class GetDeliveriesRequest
{
    public DeliveryStatusFilter? Status { get; init; }

    public int Limit { get; init; } = 50;
}
