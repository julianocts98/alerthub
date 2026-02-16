using System.Text.Json;
using AlertHub.Application.Common;
using AlertHub.Domain.Common;
using AlertHub.Infrastructure.Persistence.Entities;
using AlertHub.Infrastructure.Persistence.Entities.Deliveries;
using AlertHub.Infrastructure.Persistence.Entities.Outbox;
using AlertHub.Infrastructure.Persistence.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AlertEntity> Alerts => Set<AlertEntity>();

    public DbSet<AlertInfoEntity> AlertInfos => Set<AlertInfoEntity>();

    public DbSet<AlertInfoCategoryEntity> AlertInfoCategories => Set<AlertInfoCategoryEntity>();

    public DbSet<AlertInfoResponseTypeEntity> AlertInfoResponseTypes => Set<AlertInfoResponseTypeEntity>();

    public DbSet<AlertInfoEventCodeEntity> AlertInfoEventCodes => Set<AlertInfoEventCodeEntity>();

    public DbSet<AlertInfoParameterEntity> AlertInfoParameters => Set<AlertInfoParameterEntity>();

    public DbSet<AlertInfoResourceEntity> AlertInfoResources => Set<AlertInfoResourceEntity>();

    public DbSet<AlertAreaEntity> AlertAreas => Set<AlertAreaEntity>();

    public DbSet<AlertAreaPolygonEntity> AlertAreaPolygons => Set<AlertAreaPolygonEntity>();

    public DbSet<AlertAreaCircleEntity> AlertAreaCircles => Set<AlertAreaCircleEntity>();

    public DbSet<AlertAreaGeoCodeEntity> AlertAreaGeoCodes => Set<AlertAreaGeoCodeEntity>();

    public DbSet<SubscriptionEntity> Subscriptions => Set<SubscriptionEntity>();

    public DbSet<SubscriptionCategoryEntity> SubscriptionCategories => Set<SubscriptionCategoryEntity>();

    public DbSet<AlertDeliveryEntity> AlertDeliveries => Set<AlertDeliveryEntity>();

    public DbSet<OutboxMessageEntity> OutboxMessages => Set<OutboxMessageEntity>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDomainEventsToOutboxMessages();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Define shadow properties for all entities at once
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            modelBuilder.Entity(entityType.Name).Property<DateTimeOffset>("CreatedAt");
            modelBuilder.Entity(entityType.Name).Property<DateTimeOffset>("UpdatedAt");
        }

        base.OnModelCreating(modelBuilder);
    }

    private void ConvertDomainEventsToOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.DomainEvents.ToList();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessageEntity
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = domainEvent.OccurredOn,
                Type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            })
            .ToList();

        OutboxMessages.AddRange(outboxMessages);
    }
}
