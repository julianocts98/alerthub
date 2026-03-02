using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertAreaEntityConfiguration : IEntityTypeConfiguration<AlertAreaEntity>
{
    public void Configure(EntityTypeBuilder<AlertAreaEntity> builder)
    {
        builder.ToTable("alert_areas");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.AlertInfoId)
            .IsRequired();

        builder.Property(a => a.AreaDescription)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasMany(a => a.Polygons)
            .WithOne(p => p.Area)
            .HasForeignKey(p => p.AlertAreaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Circles)
            .WithOne(c => c.Area)
            .HasForeignKey(c => c.AlertAreaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.GeoCodes)
            .WithOne(g => g.Area)
            .HasForeignKey(g => g.AlertAreaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
