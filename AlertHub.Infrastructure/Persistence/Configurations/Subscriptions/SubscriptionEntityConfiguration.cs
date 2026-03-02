using AlertHub.Infrastructure.Persistence.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations.Subscriptions;

public sealed class SubscriptionEntityConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Channel)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(s => s.Target)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.MinSeverity)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.IsActive);

        builder.HasMany(s => s.Categories)
            .WithOne(c => c.Subscription)
            .HasForeignKey(c => c.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
