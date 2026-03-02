using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert.Events;

public record AlertIngestedDomainEvent(Guid AlertId, string Identifier, string Sender, DateTimeOffset Sent) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
