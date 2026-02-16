namespace AlertHub.Application.Deliveries;

public enum DeliveryStatusFilter
{
    Pending = 1,
    Sent = 2,
    Failed = 3
}

public sealed record DeliveryListItem(
    Guid Id,
    Guid AlertId,
    Guid SubscriptionId,
    string Target,
    string Channel,
    string Status,
    string? ExternalReference,
    string? Error,
    int RetryCount,
    DateTimeOffset? SentAtUtc,
    DateTimeOffset CreatedAt);

public enum RetryDeliveryResult
{
    Retried = 1,
    NotFound = 2,
    NotFailed = 3
}
