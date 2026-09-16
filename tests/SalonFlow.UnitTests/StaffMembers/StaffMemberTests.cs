using SalonFlow.Domain.StaffMembers;

namespace SalonFlow.UnitTests.StaffMembers;

public sealed class StaffMemberTests
{
    [Fact]
    public void CreateCreatesActiveStaffMember()
    {
        var salonId = Guid.NewGuid();

        var staffMember = StaffMember.Create(
            salonId,
            "  Anna Nguyen  ",
            "714-555-1234",
            "anna@example.com");

        Assert.NotEqual(Guid.Empty, staffMember.Id);
        Assert.Equal(salonId, staffMember.SalonId);
        Assert.Equal("Anna Nguyen", staffMember.Name);
        Assert.Equal("714-555-1234", staffMember.PhoneNumber);
        Assert.Equal("anna@example.com", staffMember.Email);
        Assert.True(staffMember.IsActive);
    }

    [Fact]
    public void CreateThrowsWhenSalonIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            StaffMember.Create(
                Guid.Empty,
                "Anna Nguyen"));

        Assert.Equal(
            "Salon ID is required. (Parameter 'salonId')",
            exception.Message);
    }

    [Fact]
    public void CreateThrowsWhenNameIsBlank()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            StaffMember.Create(
                Guid.NewGuid(),
                "   "));

        Assert.Equal(
            "Staff member name is required. (Parameter 'name')",
            exception.Message);
    }

    [Fact]
    public void DeactivateMakesStaffMemberInactive()
    {
        var staffMember = StaffMember.Create(
            Guid.NewGuid(),
            "Anna Nguyen");

        staffMember.Deactivate();

        Assert.False(staffMember.IsActive);
    }

    [Fact]
    public void ActivateMakesStaffMemberActiveAgain()
    {
        var staffMember = StaffMember.Create(
            Guid.NewGuid(),
            "Anna Nguyen");

        staffMember.Deactivate();
        staffMember.Activate();

        Assert.True(staffMember.IsActive);
    }
}
