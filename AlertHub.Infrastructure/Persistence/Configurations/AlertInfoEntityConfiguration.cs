using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertInfoEntityConfiguration : IEntityTypeConfiguration<AlertInfoEntity>
{
    public void Configure(EntityTypeBuilder<AlertInfoEntity> builder)
    {
        builder.ToTable("alert_infos");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.AlertId)
            .IsRequired();

        builder.Property(i => i.Event)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.Urgency)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(i => i.Severity)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(i => i.Certainty)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(i => i.Language)
            .HasMaxLength(35);

        builder.Property(i => i.Audience)
            .HasMaxLength(1000);

        builder.Property(i => i.SenderName)
            .HasMaxLength(500);

        builder.Property(i => i.Headline)
            .HasMaxLength(1000);

        builder.Property(i => i.Web)
            .HasMaxLength(2000);

        builder.Property(i => i.Contact)
            .HasMaxLength(1000);

        builder.HasIndex(i => i.Event);
        builder.HasIndex(i => i.Urgency);
        builder.HasIndex(i => i.Severity);
        builder.HasIndex(i => i.Certainty);
        builder.HasIndex(i => i.Expires);

        builder.HasMany(i => i.Categories)
            .WithOne(c => c.Info)
            .HasForeignKey(c => c.AlertInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.ResponseTypes)
            .WithOne(r => r.Info)
            .HasForeignKey(r => r.AlertInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.EventCodes)
            .WithOne(e => e.Info)
            .HasForeignKey(e => e.AlertInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Parameters)
            .WithOne(p => p.Info)
            .HasForeignKey(p => p.AlertInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Resources)
            .WithOne(r => r.Info)
            .HasForeignKey(r => r.AlertInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Areas)
            .WithOne(a => a.Info)
            .HasForeignKey(a => a.AlertInfoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
