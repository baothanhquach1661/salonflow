using SalonFlow.Domain.Appointments;

namespace SalonFlow.Infrastructure.Persistence.Appointments;

public sealed class AppointmentDetails
{
    private AppointmentDetails()
    {
    }

    public AppointmentDetails(
        Guid appointmentId,
        string customerName,
        string phoneNumber,
        string serviceName,
        string? staffMemberName,
        string? notes)
    {
        if (appointmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Appointment ID is required.",
                nameof(appointmentId));
        }

        AppointmentId = appointmentId;
        CustomerName = customerName.Trim();
        PhoneNumber = phoneNumber.Trim();
        ServiceName = serviceName.Trim();
        StaffMemberName = string.IsNullOrWhiteSpace(staffMemberName)
            ? null
            : staffMemberName.Trim();
        Notes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();
    }

    public Guid AppointmentId { get; private set; }

    public Appointment Appointment { get; private set; } = null!;

    public string CustomerName { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string ServiceName { get; private set; } = string.Empty;

    public string? StaffMemberName { get; private set; }

    public string? Notes { get; private set; }
}
