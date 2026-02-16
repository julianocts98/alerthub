using AlertHub.Domain.Common;

namespace AlertHub.Domain.Subscriptions.Events;

public record SubscriptionCreatedDomainEvent(Guid SubscriptionId, string UserId, string Target) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
