using System.Text.Json;
using System.Diagnostics;
using AlertHub.Infrastructure.Telemetry;
using AlertHub.Application.Alerts.Matching;
using AlertHub.Application.Common.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AlertHub.Infrastructure.BackgroundJobs;

public sealed class SubscriptionMatcherWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionMatcherWorker> _logger;
    private readonly IConfiguration _configuration;

    public SubscriptionMatcherWorker(IServiceProvider serviceProvider, ILogger<SubscriptionMatcherWorker> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunWorkerAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Subscription Matcher Worker connection failed. Retrying in 10 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private async Task RunWorkerAsync(CancellationToken stoppingToken)
    {
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
        var queueName = await channel.QueueDeclareAsync(queue: "subscription-matcher-queue", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(queue: queueName.QueueName, exchange: "alerts-exchange", routingKey: string.Empty, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            using var activity = TelemetryConstants.ActivitySource.StartActivity("ProcessSubscriptionMatching");
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                // We don't know the exact type here easily because the outbox stores it as string
                // But we know it contains an AlertId
                using var doc = JsonDocument.Parse(message);
                if (doc.RootElement.TryGetProperty("AlertId", out var alertIdProp))
                {
                    var alertId = alertIdProp.GetGuid();

                    using var scope = _serviceProvider.CreateScope();
                    var matcher = scope.ServiceProvider.GetRequiredService<AlertSubscriptionMatcher>();
                    await matcher.MatchAndScheduleAsync(alertId, stoppingToken);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message for subscription matching.");
                await channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queue: queueName.QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        _logger.LogInformation("Subscription Matcher Worker waiting for messages.");

        // Wait until cancelled
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
