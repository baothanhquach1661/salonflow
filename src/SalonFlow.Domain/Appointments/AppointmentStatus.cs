namespace SalonFlow.Domain.Appointments;

public enum AppointmentStatus
{
    Unknown = 0,
    Scheduled = 1,
    CheckedIn = 2,
    InService = 3,
    Completed = 4,
    Cancelled = 5,
    NoShow = 6
}
