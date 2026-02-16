using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public sealed record CreateSubscriptionRequest(
    SubscriptionChannel Channel,
    string Target,
    AlertSeverity? MinSeverity = null,
    List<AlertInfoCategory>? Categories = null);
