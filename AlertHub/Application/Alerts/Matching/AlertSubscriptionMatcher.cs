using AlertHub.Application.Common;
using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Entities.Deliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlertHub.Application.Alerts.Matching;

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

        // Simple matching logic: 
        // 1. Get active subscriptions.
        // 2. Filter by Severity (if specified).
        // 3. Filter by Category (if any overlap).
        
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
                    await ScheduleDeliveryAsync(alert.Id, sub, info, ct);
                    break; // One match per alert-info-subscription trio is enough
                }
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    private bool IsMatch(AlertHub.Infrastructure.Persistence.Entities.AlertInfoEntity info, AlertHub.Infrastructure.Persistence.Entities.Subscriptions.SubscriptionEntity sub)
    {
        // 1. Severity check
        if (!string.IsNullOrEmpty(sub.MinSeverity))
        {
            // Simple string comparison for now, could be enum-based weight
            if (!IsSeverityMatch(info.Severity, sub.MinSeverity)) return false;
        }

        // 2. Category check
        if (sub.Categories.Any())
        {
            var alertCategories = info.Categories.Select(c => c.Category).ToList();
            var subCategories = sub.Categories.Select(c => c.Category).ToList();
            if (!alertCategories.Intersect(subCategories).Any()) return false;
        }

        return true;
    }

    private bool IsSeverityMatch(string alertSeverity, string minSeverity)
    {
        // Placeholder for real severity hierarchy logic
        return true; 
    }

    private async Task ScheduleDeliveryAsync(Guid alertId, AlertHub.Infrastructure.Persistence.Entities.Subscriptions.SubscriptionEntity sub, AlertHub.Infrastructure.Persistence.Entities.AlertInfoEntity info, CancellationToken ct)
    {
        var alreadyScheduled = await _dbContext.AlertDeliveries
            .AnyAsync(d => d.AlertId == alertId && d.SubscriptionId == sub.Id, ct);

        if (alreadyScheduled) return;

        _dbContext.AlertDeliveries.Add(new AlertDeliveryEntity
        {
            Id = Guid.NewGuid(),
            AlertId = alertId,
            SubscriptionId = sub.Id,
            Target = sub.Target,
            Channel = sub.Channel,
            Status = DeliveryStatus.Pending,
            RetryCount = 0
        });

        _logger.LogInformation("Scheduled delivery for alert {AlertId} to subscription {SubscriptionId}", alertId, sub.Id);
    }
}
