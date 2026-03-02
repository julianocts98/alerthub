namespace AlertHub.Application.Subscriptions;

public interface ISubscriptionQueries
{
    Task<SubscriptionResponse?> GetByIdAsync(Guid id, CancellationToken ct);
}
