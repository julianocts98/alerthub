using AlertHub.Application.Subscriptions;
using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;
using AlertHub.Infrastructure.Persistence.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure.Persistence.Subscriptions;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _dbContext;

    public SubscriptionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Subscription subscription, CancellationToken ct)
    {
        var entity = new SubscriptionEntity
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            Channel = subscription.Channel,
            Target = subscription.Target,
            IsActive = subscription.IsActive,
            MinSeverity = subscription.MinSeverity,
            Categories = subscription.Categories
                .Select(c => new SubscriptionCategoryEntity { Id = Guid.NewGuid(), SubscriptionId = subscription.Id, Category = c })
                .ToList()
        };

        _dbContext.Subscriptions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Subscriptions
            .Include(s => s.Categories)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<IReadOnlyCollection<Subscription>> GetActiveByCriteriaAsync(
        AlertSeverity severity,
        IEnumerable<AlertInfoCategory> categories,
        CancellationToken ct)
    {
        var categoryList = categories.ToList();

        var entities = await _dbContext.Subscriptions
            .AsNoTracking()
            .Include(s => s.Categories)
            .Where(s => s.IsActive)
            .Where(s => s.MinSeverity == null || s.MinSeverity <= severity)
            .Where(s => !s.Categories.Any() || s.Categories.Any(c => categoryList.Contains(c.Category)))
            .ToListAsync(ct);

        return entities.Select(MapToDomain).ToList();
    }

    private static Subscription MapToDomain(SubscriptionEntity entity)
    {
        return new Subscription(
            entity.Id,
            entity.UserId,
            entity.Channel,
            entity.Target,
            entity.IsActive,
            entity.MinSeverity,
            entity.Categories.Select(c => c.Category));
    }
}
