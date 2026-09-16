using SalonFlow.Domain.Appointments;

namespace SalonFlow.UnitTests.Appointments;

public sealed class AppointmentTests
{
    private static readonly DateTimeOffset UtcNow =
        new(2026, 9, 14, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ScheduleCreatesAppointmentWhenDataIsValid()
    {
        // Arrange
        var salonId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var startsAtUtc = UtcNow.AddHours(1);
        var duration = TimeSpan.FromMinutes(45);

        // Act
        var appointment = Appointment.Schedule(
            salonId,
            customerId,
            serviceId,
            startsAtUtc,
            duration,
            UtcNow);

        // Assert
        Assert.NotEqual(Guid.Empty, appointment.Id);
        Assert.Equal(salonId, appointment.SalonId);
        Assert.Equal(customerId, appointment.CustomerId);
        Assert.Equal(serviceId, appointment.ServiceId);
        Assert.Null(appointment.StaffMemberId);
        Assert.Equal(startsAtUtc, appointment.StartsAtUtc);
        Assert.Equal(duration, appointment.Duration);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        Assert.Equal(startsAtUtc.Add(duration), appointment.EndsAtUtc);
    }

    [Fact]
    public void ScheduleThrowsWhenSalonIdIsEmpty()
    {
        // Arrange
        var salonId = Guid.Empty;
        var customerId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var startsAtUtc = UtcNow.AddHours(1);
        var duration = TimeSpan.FromMinutes(45);

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Appointment.Schedule(
                salonId,
                customerId,
                serviceId,
                startsAtUtc,
                duration,
                UtcNow));

        // Assert
        Assert.Equal(nameof(salonId), exception.ParamName);
    }

    [Fact]
    public void ScheduleThrowsWhenCustomerIdIsEmpty()
    {
        // Arrange
        var salonId = Guid.NewGuid();
        var customerId = Guid.Empty;
        var serviceId = Guid.NewGuid();
        var startsAtUtc = UtcNow.AddHours(1);
        var duration = TimeSpan.FromMinutes(45);

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Appointment.Schedule(
                salonId,
                customerId,
                serviceId,
                startsAtUtc,
                duration,
                UtcNow));

        // Assert
        Assert.Equal(nameof(customerId), exception.ParamName);
    }

    [Fact]
    public void ScheduleThrowsWhenServiceIdIsEmpty()
    {
        // Arrange
        var salonId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var serviceId = Guid.Empty;
        var startsAtUtc = UtcNow.AddHours(1);
        var duration = TimeSpan.FromMinutes(45);

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Appointment.Schedule(
                salonId,
                customerId,
                serviceId,
                startsAtUtc,
                duration,
                UtcNow));

        // Assert
        Assert.Equal(nameof(serviceId), exception.ParamName);
    }

    [Fact]
    public void ScheduleThrowsWhenStaffMemberIdIsEmpty()
    {
        // Arrange
        var salonId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var staffMemberId = Guid.Empty;
        var startsAtUtc = UtcNow.AddHours(1);
        var duration = TimeSpan.FromMinutes(45);

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Appointment.Schedule(
                salonId,
                customerId,
                serviceId,
                startsAtUtc,
                duration,
                UtcNow,
                staffMemberId));

        // Assert
        Assert.Equal(nameof(staffMemberId), exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public void ScheduleThrowsWhenDurationIsNotPositive(
        int durationMinutes)
    {
        // Arrange
        var salonId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var startsAtUtc = UtcNow.AddHours(1);
        var duration = TimeSpan.FromMinutes(durationMinutes);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            Appointment.Schedule(
                salonId,
                customerId,
                serviceId,
                startsAtUtc,
                duration,
                UtcNow));

        // Assert
        Assert.Equal(nameof(duration), exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-60)]
    public void ScheduleThrowsWhenStartTimeIsNotInFuture(
        int minutesFromNow)
    {
        // Arrange
        var salonId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var startsAtUtc = UtcNow.AddMinutes(minutesFromNow);
        var duration = TimeSpan.FromMinutes(45);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            Appointment.Schedule(
                salonId,
                customerId,
                serviceId,
                startsAtUtc,
                duration,
                UtcNow));

        // Assert
        Assert.Equal(nameof(startsAtUtc), exception.ParamName);
    }

    [Fact]
    public void CheckInChangesStatusFromScheduledToCheckedIn()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        // Act
        appointment.CheckIn();

        // Assert
        Assert.Equal(AppointmentStatus.CheckedIn, appointment.Status);
    }

    [Fact]
    public void StartServiceChangesStatusFromCheckedInToInService()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        appointment.CheckIn();

        // Act
        appointment.StartService();

        // Assert
        Assert.Equal(AppointmentStatus.InService, appointment.Status);
    }

    [Fact]
    public void StartServiceThrowsWhenAppointmentIsScheduled()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            appointment.StartService);

        // Assert
        Assert.Equal(
            "Only checked-in appointments can start service. Current status: Scheduled.",
            exception.Message);
    }

    [Fact]
    public void CheckInThrowsWhenAppointmentIsAlreadyCheckedIn()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        appointment.CheckIn();

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            appointment.CheckIn);

        // Assert
        Assert.Equal(
            "Only scheduled appointments can be checked in. Current status: CheckedIn.",
            exception.Message);
    }

    [Fact]
    public void CompleteChangesStatusFromInServiceToCompleted()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        appointment.CheckIn();
        appointment.StartService();

        // Act
        appointment.Complete();

        // Assert
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void CompleteThrowsWhenAppointmentIsCheckedIn()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        appointment.CheckIn();

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            appointment.Complete);

        // Assert
        Assert.Equal(
            "Only appointments in service can be completed. Current status: CheckedIn.",
            exception.Message);
    }

    [Fact]
    public void CancelChangesStatusFromScheduledToCancelled()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        // Act
        appointment.Cancel();

        // Assert
        Assert.Equal(
            AppointmentStatus.Cancelled,
            appointment.Status);
    }

    [Fact]
    public void CancelChangesStatusFromCheckedInToCancelled()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        appointment.CheckIn();

        // Act
        appointment.Cancel();

        // Assert
        Assert.Equal(
            AppointmentStatus.Cancelled,
            appointment.Status);
    }

    [Fact]
    public void CancelThrowsWhenAppointmentIsInService()
    {
        // Arrange
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow.AddHours(1),
            TimeSpan.FromMinutes(45),
            UtcNow);

        appointment.CheckIn();
        appointment.StartService();

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            appointment.Cancel);

        // Assert
        Assert.Equal(
            "Only scheduled or checked-in appointments can be cancelled. Current status: InService.",
            exception.Message);
    }
}



