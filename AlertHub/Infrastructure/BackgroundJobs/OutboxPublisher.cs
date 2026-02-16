using System.Text.Json;
using System.Diagnostics;
using AlertHub.Infrastructure.Telemetry;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Entities.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace AlertHub.Infrastructure.BackgroundJobs;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(IServiceProvider serviceProvider, ILogger<OutboxPublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Publisher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while publishing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task PublishOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        using var activity = TelemetryConstants.ActivitySource.StartActivity("PublishOutboxMessages");
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(stoppingToken);

        if (messages.Count == 0) return;

        // In a real app, we'd use a dedicated RabbitMQ service/connection pool
        // For simplicity, we create a connection here
        var factory = new ConnectionFactory { HostName = "localhost", UserName = "alerthub", Password = "alerthub" };
        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(exchange: "alerts-exchange", type: ExchangeType.Fanout, durable: true, cancellationToken: stoppingToken);

        foreach (var message in messages)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message.Content);

                await channel.BasicPublishAsync(
                    exchange: "alerts-exchange",
                    routingKey: string.Empty,
                    body: body,
                    cancellationToken: stoppingToken);

                message.ProcessedOnUtc = DateTimeOffset.UtcNow;
                _logger.LogInformation("Published outbox message {MessageId} of type {Type}", message.Id, message.Type);
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;
                _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
