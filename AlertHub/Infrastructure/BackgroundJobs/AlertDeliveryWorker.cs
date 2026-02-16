using AlertHub.Application.Common.Delivery;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Entities.Deliveries;
using AlertHub.Infrastructure.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure.BackgroundJobs;

public sealed class AlertDeliveryWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlertDeliveryWorker> _logger;
    private readonly IConfiguration _configuration;

    public AlertDeliveryWorker(IServiceProvider serviceProvider, ILogger<AlertDeliveryWorker> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Alert Delivery Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDeliveriesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing alert deliveries.");
            }

            var delayMs = int.Parse(_configuration["BackgroundJobs:DeliveryIntervalMs"] ?? "5000");
            await Task.Delay(TimeSpan.FromMilliseconds(delayMs), stoppingToken);
        }
    }

    private async Task ProcessDeliveriesAsync(CancellationToken stoppingToken)
    {
        using var activity = TelemetryConstants.ActivitySource.StartActivity("ProcessAlertDeliveries");
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deliveryChannels = scope.ServiceProvider.GetServices<IAlertDeliveryChannel>();

        var pendingDeliveries = await dbContext.AlertDeliveries
            .Where(d => d.Status == DeliveryStatus.Pending && d.RetryCount < 3)
            .Take(10)
            .ToListAsync(stoppingToken);

        if (pendingDeliveries.Count == 0) return;

        foreach (var delivery in pendingDeliveries)
        {
            var channel = deliveryChannels.FirstOrDefault(c => c.SupportedChannel.ToString() == delivery.Channel);

            if (channel == null)
            {
                _logger.LogWarning("No delivery channel found for {Channel}", delivery.Channel);
                delivery.Status = DeliveryStatus.Failed;
                delivery.Error = "Unsupported channel";
                continue;
            }

            try
            {
                // In a real scenario, we'd fetch the alert content here to build the message
                var result = await channel.SendAsync(new DeliveryRequest(
                    delivery.Target,
                    $"New Alert received (ID: {delivery.AlertId})"), stoppingToken);

                if (result.IsSuccess)
                {
                    delivery.Status = DeliveryStatus.Sent;
                    delivery.SentAtUtc = DateTimeOffset.UtcNow;
                    delivery.ExternalReference = result.ExternalReference;
                    _logger.LogInformation("Successfully sent alert {AlertId} via {Channel} to {Target}",
                        delivery.AlertId, delivery.Channel, delivery.Target);
                }
                else
                {
                    delivery.RetryCount++;
                    delivery.Error = result.ErrorMessage;
                    if (delivery.RetryCount >= 3) delivery.Status = DeliveryStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                delivery.RetryCount++;
                delivery.Error = ex.Message;
                if (delivery.RetryCount >= 3) delivery.Status = DeliveryStatus.Failed;
                _logger.LogError(ex, "Failed to send alert delivery {DeliveryId}", delivery.Id);
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
