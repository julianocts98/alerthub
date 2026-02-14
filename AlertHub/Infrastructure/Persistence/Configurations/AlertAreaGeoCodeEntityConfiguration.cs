using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertAreaGeoCodeEntityConfiguration : IEntityTypeConfiguration<AlertAreaGeoCodeEntity>
{
    public void Configure(EntityTypeBuilder<AlertAreaGeoCodeEntity> builder)
    {
        builder.ToTable("alert_area_geocodes");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id)
            .ValueGeneratedNever();

        builder.Property(g => g.ValueName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(g => g.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(g => new { g.ValueName, g.Value });
    }
}
