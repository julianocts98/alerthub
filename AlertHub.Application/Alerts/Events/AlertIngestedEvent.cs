namespace AlertHub.Application.Alerts.Events;

public record AlertIngestedEvent(Guid AlertId, DateTimeOffset OccurredOnUtc);
