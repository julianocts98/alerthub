using AlertHub.Domain.Common;

namespace AlertHub.Application.Subscriptions;

public interface ISubscriptionService
{
    Task<Result<SubscriptionResponse>> CreateSubscriptionAsync(CreateSubscriptionRequest request, string userId, CancellationToken ct);
    Task<Result<SubscriptionResponse>> GetByIdAsync(Guid id, CancellationToken ct);
}
