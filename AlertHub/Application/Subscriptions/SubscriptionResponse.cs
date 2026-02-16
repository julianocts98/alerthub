using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public sealed record SubscriptionResponse(
    Guid Id,
    string UserId,
    SubscriptionChannel Channel,
    string Target,
    bool IsActive,
    AlertSeverity? MinSeverity,
    List<AlertInfoCategory> Categories);
