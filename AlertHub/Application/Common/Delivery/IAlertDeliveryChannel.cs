using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Common.Delivery;

public record DeliveryRequest(
    string Target,
    string Content,
    string? Subject = null);

public record DeliveryResult(bool IsSuccess, string? ErrorMessage = null, string? ExternalReference = null);

public interface IAlertDeliveryChannel
{
    SubscriptionChannel SupportedChannel { get; }
    Task<DeliveryResult> SendAsync(DeliveryRequest request, CancellationToken ct);
}
