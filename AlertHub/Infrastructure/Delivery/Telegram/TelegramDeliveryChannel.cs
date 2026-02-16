using AlertHub.Application.Common.Delivery;
using AlertHub.Domain.Subscriptions;
using Telegram.Bot;

namespace AlertHub.Infrastructure.Delivery.Telegram;

public sealed class TelegramDeliveryChannel : IAlertDeliveryChannel
{
    private readonly ILogger<TelegramDeliveryChannel> _logger;
    private readonly ITelegramBotClient? _botClient;

    public TelegramDeliveryChannel(ILogger<TelegramDeliveryChannel> logger, IConfiguration configuration)
    {
        _logger = logger;
        var token = configuration["Telegram:BotToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _botClient = new TelegramBotClient(token);
        }
    }

    public SubscriptionChannel SupportedChannel => SubscriptionChannel.Telegram;

    public async Task<DeliveryResult> SendAsync(DeliveryRequest request, CancellationToken ct)
    {
        if (_botClient == null)
        {
            _logger.LogWarning("Telegram Bot Token is not configured. Simulating success.");
            return new DeliveryResult(true, ExternalReference: "SIMULATED_" + Guid.NewGuid());
        }

        try
        {
            var message = await _botClient.SendMessage(
                chatId: request.Target,
                text: request.Content,
                cancellationToken: ct);

            return new DeliveryResult(true, ExternalReference: message.Id.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Telegram message to {Target}", request.Target);
            return new DeliveryResult(false, ex.Message);
        }
    }
}

