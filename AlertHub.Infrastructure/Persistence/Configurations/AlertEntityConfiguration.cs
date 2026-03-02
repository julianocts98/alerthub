using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertEntityConfiguration : IEntityTypeConfiguration<AlertEntity>
{
    public void Configure(EntityTypeBuilder<AlertEntity> builder)
    {
        builder.ToTable("alerts");

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

        builder.Property(a => a.Source)
            .HasMaxLength(500);

        builder.Property(a => a.Restriction)
            .HasMaxLength(1000);

        builder.Property(a => a.Note)
            .HasMaxLength(4000);

        builder.Property(a => a.Addresses)
            .HasMaxLength(4000);

        builder.Property(a => a.Codes)
            .HasMaxLength(2000);

        builder.Property(a => a.References)
            .HasMaxLength(4000);

        builder.Property(a => a.Incidents)
            .HasMaxLength(2000);

        builder.Property(a => a.RawPayload)
            .IsRequired();

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.IngestedAtUtc)
            .IsRequired();

        builder.HasIndex(a => new { a.Sender, a.Identifier })
            .IsUnique();

        builder.HasMany(a => a.Infos)
            .WithOne(i => i.Alert)
            .HasForeignKey(i => i.AlertId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
