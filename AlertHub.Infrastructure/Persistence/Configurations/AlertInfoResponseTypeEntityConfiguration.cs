using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertInfoResponseTypeEntityConfiguration : IEntityTypeConfiguration<AlertInfoResponseTypeEntity>
{
    public void Configure(EntityTypeBuilder<AlertInfoResponseTypeEntity> builder)
    {
        builder.ToTable("alert_info_response_types");

        builder.HasKey(r => new { r.AlertInfoId, r.ResponseType });

        builder.Property(r => r.ResponseType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);
    }
}
