using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SalonFlow.Domain.Services;

namespace SalonFlow.Infrastructure.Persistence.Services;

internal sealed class SalonServiceConfiguration
    : IEntityTypeConfiguration<SalonService>
{
    public void Configure(
        EntityTypeBuilder<SalonService> builder)
    {
        builder.ToTable("SalonServices");

        builder.HasKey(service => service.Id);

        builder.Property(service => service.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(service => service.Duration)
            .HasConversion(
                duration => duration.Ticks,
                ticks => TimeSpan.FromTicks(ticks))
            .IsRequired();

        builder.Property(service => service.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(service => service.IsActive)
            .IsRequired();

        builder.HasIndex(service => new
        {
            service.SalonId,
            service.Name
        }).IsUnique();

        builder.HasIndex(service => new
        {
            service.SalonId,
            service.IsActive
        });
    }
}
