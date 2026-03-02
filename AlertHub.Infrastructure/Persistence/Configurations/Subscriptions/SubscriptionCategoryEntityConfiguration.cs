using AlertHub.Infrastructure.Persistence.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations.Subscriptions;

public sealed class SubscriptionCategoryEntityConfiguration : IEntityTypeConfiguration<SubscriptionCategoryEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionCategoryEntity> builder)
    {
        builder.ToTable("subscription_categories");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Category)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(c => new { c.SubscriptionId, c.Category })
            .IsUnique();
    }
}
