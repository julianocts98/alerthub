using AlertHub.Application.Common;

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

    public async Task<Result> RetryDeliveryAsync(Guid id, CancellationToken ct)
    {
        var result = await _repository.RetryFailedAsync(id, ct);
        return result switch
        {
            RetryDeliveryResult.Retried => Result.Success(),
            RetryDeliveryResult.NotFound => Result.Failure(
                new ResultError("delivery.not_found", $"Delivery with ID '{id}' was not found.")),
            RetryDeliveryResult.NotFailed => Result.Failure(
                new ResultError("delivery.invalid_state", "Only failed deliveries can be retried.")),
            _ => Result.Failure(
                new ResultError("delivery.retry_failed", "An unexpected delivery retry error occurred."))
        };
    }
}
