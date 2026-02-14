using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertInfoCategoryEntityConfiguration : IEntityTypeConfiguration<AlertInfoCategoryEntity>
{
    public void Configure(EntityTypeBuilder<AlertInfoCategoryEntity> builder)
    {
        builder.ToTable("alert_info_categories");

        builder.HasKey(c => new { c.AlertInfoId, c.Category });

        builder.Property(c => c.Category)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(c => c.Category);
    }
}
