using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public interface ISubscriptionRepository
{
    Task AddAsync(Subscription subscription, CancellationToken ct);
}
