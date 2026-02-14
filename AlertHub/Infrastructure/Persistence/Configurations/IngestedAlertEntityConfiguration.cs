using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class IngestedAlertEntityConfiguration : IEntityTypeConfiguration<IngestedAlertEntity>
{
    public void Configure(EntityTypeBuilder<IngestedAlertEntity> builder)
    {
        builder.ToTable("ingested_alerts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.Identifier)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Sender)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Sent)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(a => a.MessageType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(a => a.Scope)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(a => a.RawPayload)
            .IsRequired();

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.IngestedAtUtc)
            .IsRequired();

        builder.HasIndex(a => new { a.Sender, a.Identifier })
            .IsUnique();
    }
}
