using System.Text;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Telemetry;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace AlertHub.Infrastructure.BackgroundJobs;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly IConfiguration _configuration;

    public OutboxPublisher(IServiceProvider serviceProvider, ILogger<OutboxPublisher> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
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

            var delayMs = int.Parse(_configuration["BackgroundJobs:OutboxIntervalMs"] ?? "5000");
            await Task.Delay(TimeSpan.FromMilliseconds(delayMs), stoppingToken);
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

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:UserName"] ?? "alerthub",
            Password = _configuration["RabbitMQ:Password"] ?? "alerthub"
        };
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
