namespace AlertHub.Infrastructure.Persistence.Entities.Deliveries;

public enum DeliveryStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3
}

public sealed class AlertDeliveryEntity
{
    public Guid Id { get; set; }
    public Guid AlertId { get; set; }
    public Guid SubscriptionId { get; set; }
    public string Target { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public DeliveryStatus Status { get; set; }
    public string? ExternalReference { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset? SentAtUtc { get; set; }
}
