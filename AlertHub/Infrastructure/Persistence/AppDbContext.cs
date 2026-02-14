using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
