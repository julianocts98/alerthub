using AlertHub.Domain.Alert;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Entities.Deliveries;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure.Alerts.Matching;

public sealed class AlertSubscriptionMatcher
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AlertSubscriptionMatcher> _logger;

    public AlertSubscriptionMatcher(AppDbContext dbContext, ILogger<AlertSubscriptionMatcher> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task MatchAndScheduleAsync(Guid alertId, CancellationToken ct)
    {
        var alert = await _dbContext.Alerts
            .Include(a => a.Infos)
                .ThenInclude(i => i.Categories)
            .FirstOrDefaultAsync(a => a.Id == alertId, ct);

        if (alert == null) return;

        var subscriptions = await _dbContext.Subscriptions
            .Include(s => s.Categories)
            .Where(s => s.IsActive)
            .ToListAsync(ct);

        foreach (var sub in subscriptions)
        {
            foreach (var info in alert.Infos)
            {
                if (IsMatch(info, sub))
                {
                    await ScheduleDeliveryAsync(alert.Id, sub.Id, sub.Target, sub.Channel.ToString(), ct);
                    break;
                }
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    private static bool IsMatch(
        AlertHub.Infrastructure.Persistence.Entities.AlertInfoEntity info,
        AlertHub.Infrastructure.Persistence.Entities.Subscriptions.SubscriptionEntity sub)
    {
        if (sub.MinSeverity.HasValue && !IsSeverityMatch(info.Severity, sub.MinSeverity.Value))
            return false;

        if (sub.Categories.Any())
        {
            var alertCategories = info.Categories.Select(c => c.Category).ToList();
            var subCategories = sub.Categories.Select(c => c.Category).ToList();
            if (!alertCategories.Intersect(subCategories).Any())
                return false;
        }

        return true;
    }

    private static bool IsSeverityMatch(AlertSeverity alertSeverity, AlertSeverity minSeverity)
        => GetSeverityWeight(alertSeverity) >= GetSeverityWeight(minSeverity);

    private static int GetSeverityWeight(AlertSeverity severity) => severity switch
    {
        AlertSeverity.Extreme => 4,
        AlertSeverity.Severe => 3,
        AlertSeverity.Moderate => 2,
        AlertSeverity.Minor => 1,
        _ => 0
    };

    private async Task ScheduleDeliveryAsync(Guid alertId, Guid subscriptionId, string target, string channel, CancellationToken ct)
    {
        var alreadyScheduled = await _dbContext.AlertDeliveries
            .AnyAsync(d => d.AlertId == alertId && d.SubscriptionId == subscriptionId, ct);

        if (alreadyScheduled) return;

        _dbContext.AlertDeliveries.Add(new AlertDeliveryEntity
        {
            Id = Guid.NewGuid(),
            AlertId = alertId,
            SubscriptionId = subscriptionId,
            Target = target,
            Channel = channel,
            Status = DeliveryStatus.Pending,
            RetryCount = 0
        });

        _logger.LogInformation("Scheduled delivery for alert {AlertId} to subscription {SubscriptionId}", alertId, subscriptionId);
    }
}
