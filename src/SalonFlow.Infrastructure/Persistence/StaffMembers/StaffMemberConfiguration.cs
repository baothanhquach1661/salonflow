using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SalonFlow.Domain.StaffMembers;

namespace SalonFlow.Infrastructure.Persistence.StaffMembers;

internal sealed class StaffMemberConfiguration
    : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.ToTable("StaffMembers");

        builder.HasKey(staffMember => staffMember.Id);

        builder.Property(staffMember => staffMember.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(staffMember => staffMember.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(staffMember => staffMember.Email)
            .HasMaxLength(200);

        builder.Property(staffMember => staffMember.IsActive)
            .IsRequired();

        builder.HasIndex(staffMember => new
        {
            staffMember.SalonId,
            staffMember.IsActive
        });
    }
}
