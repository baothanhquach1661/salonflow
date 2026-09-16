using Microsoft.EntityFrameworkCore;

using SalonFlow.Domain.Appointments;
using SalonFlow.Domain.Services;
using SalonFlow.Domain.StaffMembers;
using SalonFlow.Infrastructure.Persistence.Appointments;
using SalonFlow.Infrastructure.Persistence.Services;
using SalonFlow.Infrastructure.Persistence.StaffMembers;

namespace SalonFlow.Infrastructure.Persistence;

public sealed class SalonFlowDbContext(
    DbContextOptions<SalonFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<SalonService> SalonServices =>
    Set<SalonService>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<Appointment> Appointments =>
        Set<Appointment>();

    public DbSet<AppointmentDetails> AppointmentDetails =>
        Set<AppointmentDetails>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(
            new SalonServiceConfiguration());

        modelBuilder.ApplyConfiguration(new StaffMemberConfiguration());

        base.OnModelCreating(modelBuilder);

        var appointment = modelBuilder.Entity<Appointment>();

        appointment.ToTable("Appointments");
        appointment.HasKey(item => item.Id);

        appointment.Property(item => item.Id)
            .ValueGeneratedNever();

        appointment.Property(item => item.StartsAtUtc)
            .HasConversion<long>(
                value => value.ToUnixTimeMilliseconds(),
                value => DateTimeOffset.FromUnixTimeMilliseconds(value));

        appointment.Property(item => item.Duration)
            .HasConversion<long>(
                value => value.Ticks,
                value => TimeSpan.FromTicks(value));

        appointment.Property(item => item.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        appointment.Ignore(item => item.EndsAtUtc);

        appointment.HasIndex(item => new
        {
            item.SalonId,
            item.StartsAtUtc
        });

        var details = modelBuilder.Entity<AppointmentDetails>();

        details.ToTable("AppointmentDetails");
        details.HasKey(item => item.AppointmentId);

        details.Property(item => item.CustomerName)
            .HasMaxLength(100)
            .IsRequired();

        details.Property(item => item.PhoneNumber)
            .HasMaxLength(30)
            .IsRequired();

        details.Property(item => item.ServiceName)
            .HasMaxLength(100)
            .IsRequired();

        details.Property(item => item.StaffMemberName)
            .HasMaxLength(100);

        details.Property(item => item.Notes)
            .HasMaxLength(500);

        details.HasOne(item => item.Appointment)
            .WithOne()
            .HasForeignKey<AppointmentDetails>(
                item => item.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
