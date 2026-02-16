namespace AlertHub.Application.Common.Messaging;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
}

public record AlertIngestedIntegrationEvent(
    Guid AlertId,
    string Identifier,
    string Sender,
    DateTimeOffset Sent,
    string Severity,
    string[] Categories) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
