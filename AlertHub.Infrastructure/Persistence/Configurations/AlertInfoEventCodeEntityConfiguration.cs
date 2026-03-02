using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertInfoEventCodeEntityConfiguration : IEntityTypeConfiguration<AlertInfoEventCodeEntity>
{
    public void Configure(EntityTypeBuilder<AlertInfoEventCodeEntity> builder)
    {
        builder.ToTable("alert_info_event_codes");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.ValueName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Value)
            .IsRequired()
            .HasMaxLength(500);
    }
}
