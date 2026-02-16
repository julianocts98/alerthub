using AlertHub.Application.Common.Delivery;
using AlertHub.Domain.Subscriptions;
using Microsoft.Extensions.Logging;

namespace AlertHub.Infrastructure.Delivery.Telegram;

public sealed class TelegramDeliveryChannel : IAlertDeliveryChannel
{
    private readonly ILogger<TelegramDeliveryChannel> _logger;

    public TelegramDeliveryChannel(ILogger<TelegramDeliveryChannel> logger)
    {
        _logger = logger;
    }

    public SubscriptionChannel SupportedChannel => SubscriptionChannel.Telegram;

    public async Task<DeliveryResult> SendAsync(DeliveryRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Sending Telegram message to {Target}: {Content}", request.Target, request.Content);
        
        // Simulating network delay
        await Task.Delay(100, ct);

        return new DeliveryResult(true, ExternalReference: Guid.NewGuid().ToString());
    }
}
