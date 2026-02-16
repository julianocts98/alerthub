using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;
using AlertHub.Domain.Common;

namespace AlertHub.Infrastructure.Persistence.Entities.Subscriptions;

public sealed class SubscriptionEntity : AggregateRoot
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public SubscriptionChannel Channel { get; set; }

    public string Target { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public AlertSeverity? MinSeverity { get; set; }

    public List<SubscriptionCategoryEntity> Categories { get; set; } = [];
}
