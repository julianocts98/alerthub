using AlertHub.Application.Subscriptions;
using AlertHub.Domain.Subscriptions;
using AlertHub.Infrastructure.Persistence.Entities.Subscriptions;

namespace AlertHub.Infrastructure.Persistence.Subscriptions;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _dbContext;

    public SubscriptionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Subscription subscription, CancellationToken ct)
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

        foreach (var @event in subscription.DomainEvents)
        {
            entity.AddDomainEvent(@event);
        }

        _dbContext.Subscriptions.Add(entity);
        return Task.CompletedTask;
    }

}
