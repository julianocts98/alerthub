using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertAreaPolygonEntityConfiguration : IEntityTypeConfiguration<AlertAreaPolygonEntity>
{
    public void Configure(EntityTypeBuilder<AlertAreaPolygonEntity> builder)
    {
        builder.ToTable("alert_area_polygons");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        // Stored as CAP text for now; migrate column to PostGIS geometry when spatial queries are added.
        builder.Property(p => p.Points)
            .IsRequired();
    }
}
