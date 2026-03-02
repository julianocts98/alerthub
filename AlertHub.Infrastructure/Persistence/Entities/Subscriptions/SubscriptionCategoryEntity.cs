using AlertHub.Domain.Alert;

namespace AlertHub.Infrastructure.Persistence.Entities.Subscriptions;

public sealed class SubscriptionCategoryEntity
{
    public Guid Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public AlertInfoCategory Category { get; set; }

    public SubscriptionEntity Subscription { get; set; } = default!;
}
