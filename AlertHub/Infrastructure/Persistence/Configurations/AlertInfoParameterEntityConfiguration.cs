using AlertHub.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertHub.Infrastructure.Persistence.Configurations;

public sealed class AlertInfoParameterEntityConfiguration : IEntityTypeConfiguration<AlertInfoParameterEntity>
{
    public void Configure(EntityTypeBuilder<AlertInfoParameterEntity> builder)
    {
        builder.ToTable("alert_info_parameters");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.ValueName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Value)
            .IsRequired()
            .HasMaxLength(4000);
    }
}
