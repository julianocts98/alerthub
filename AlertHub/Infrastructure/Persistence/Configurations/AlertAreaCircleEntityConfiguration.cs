using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertAreaCircleEntityConfiguration : IEntityTypeConfiguration<AlertAreaCircleEntity>
{
    public void Configure(EntityTypeBuilder<AlertAreaCircleEntity> builder)
    {
        builder.ToTable("alert_area_circles");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.CenterLatitude)
            .IsRequired();

        builder.Property(c => c.CenterLongitude)
            .IsRequired();

        builder.Property(c => c.Radius)
            .IsRequired();
    }
}
