using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertInfoResourceEntityConfiguration : IEntityTypeConfiguration<AlertInfoResourceEntity>
{
    public void Configure(EntityTypeBuilder<AlertInfoResourceEntity> builder)
    {
        builder.ToTable("alert_info_resources");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.ResourceDescription)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.MimeType)
            .HasMaxLength(256);

        builder.Property(r => r.Uri)
            .HasMaxLength(2000);

        builder.Property(r => r.Digest)
            .HasMaxLength(200);
    }
}
