namespace SalonFlow.Domain.StaffMembers;

public sealed class StaffMember
{
    private StaffMember(
        Guid id,
        Guid salonId,
        string name,
        string? phoneNumber,
        string? email)
    {
        Id = id;
        SalonId = salonId;
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid SalonId { get; private set; }

    public string Name { get; private set; }

    public string? PhoneNumber { get; private set; }

    public string? Email { get; private set; }

    public bool IsActive { get; private set; }

    public static StaffMember Create(
        Guid salonId,
        string name,
        string? phoneNumber = null,
        string? email = null)
    {
        if (salonId == Guid.Empty)
        {
            throw new ArgumentException(
                "Salon ID is required.",
                nameof(salonId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Staff member name is required.",
                nameof(name));
        }

        return new StaffMember(
            Guid.NewGuid(),
            salonId,
            name.Trim(),
            string.IsNullOrWhiteSpace(phoneNumber)
                ? null
                : phoneNumber.Trim(),
            string.IsNullOrWhiteSpace(email)
                ? null
                : email.Trim());
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
