using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public interface ISubscriptionRepository
{
    Task AddAsync(Subscription subscription, CancellationToken ct);
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<Subscription>> GetActiveByCriteriaAsync(
        AlertSeverity severity,
        IEnumerable<AlertInfoCategory> categories,
        CancellationToken ct);
}
