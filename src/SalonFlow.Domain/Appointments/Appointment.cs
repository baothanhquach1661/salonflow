namespace SalonFlow.Domain.Appointments;

public sealed class Appointment
{
    private Appointment(
        Guid id,
        Guid salonId,
        Guid customerId,
        Guid serviceId,
        Guid? staffMemberId,
        DateTimeOffset startsAtUtc,
        TimeSpan duration)
    {
        Id = id;
        SalonId = salonId;
        CustomerId = customerId;
        ServiceId = serviceId;
        StaffMemberId = staffMemberId;
        StartsAtUtc = startsAtUtc.ToUniversalTime();
        Duration = duration;
        Status = AppointmentStatus.Scheduled;
    }

    public Guid Id { get; private set; }

    public Guid SalonId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid ServiceId { get; private set; }

    public Guid? StaffMemberId { get; private set; }

    public DateTimeOffset StartsAtUtc { get; private set; }

    public TimeSpan Duration { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public DateTimeOffset EndsAtUtc => StartsAtUtc.Add(Duration);

    public static Appointment Schedule(
    Guid salonId,
    Guid customerId,
    Guid serviceId,
    DateTimeOffset startsAtUtc,
    TimeSpan duration,
    DateTimeOffset utcNow,
    Guid? staffMemberId = null)
    {
        if (salonId == Guid.Empty)
        {
            throw new ArgumentException("Salon ID is required.", nameof(salonId));
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID is required.", nameof(customerId));
        }

        if (serviceId == Guid.Empty)
        {
            throw new ArgumentException("Service ID is required.", nameof(serviceId));
        }

        if (staffMemberId.HasValue && staffMemberId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "Staff member ID cannot be empty.",
                nameof(staffMemberId));
        }

        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(duration),
                duration,
                "Duration must be greater than zero.");
        }

        if (startsAtUtc <= utcNow)
        {
            throw new ArgumentOutOfRangeException(
                nameof(startsAtUtc),
                startsAtUtc,
                "Appointment must start in the future.");
        }

        return new Appointment(
            Guid.NewGuid(),
            salonId,
            customerId,
            serviceId,
            staffMemberId,
            startsAtUtc,
            duration);
    }


    public void CheckIn()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException(
                $"Only scheduled appointments can be checked in. Current status: {Status}.");
        }

        Status = AppointmentStatus.CheckedIn;
    }

    public void StartService()
    {
        if (Status != AppointmentStatus.CheckedIn)
        {
            throw new InvalidOperationException(
                $"Only checked-in appointments can start service. Current status: {Status}.");
        }

        Status = AppointmentStatus.InService;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.InService)
        {
            throw new InvalidOperationException(
                $"Only appointments in service can be completed. Current status: {Status}.");
        }

        Status = AppointmentStatus.Completed;
    }

    public void Cancel()
    {
        if (Status != AppointmentStatus.Scheduled &&
            Status != AppointmentStatus.CheckedIn)
        {
            throw new InvalidOperationException(
                $"Only scheduled or checked-in appointments can be cancelled. Current status: {Status}.");
        }

        Status = AppointmentStatus.Cancelled;
    }
}

