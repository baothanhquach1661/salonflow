using Microsoft.AspNetCore.Mvc;

using SalonFlow.Domain.Appointments;

namespace SalonFlow.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public sealed class AppointmentsController : ControllerBase
{
    [HttpPost]
    public ActionResult<AppointmentResponse> Schedule(
        ScheduleAppointmentRequest request)
    {
        try
        {
            var appointment = Appointment.Schedule(
                request.SalonId,
                request.CustomerId,
                request.ServiceId,
                request.StartsAtUtc,
                TimeSpan.FromMinutes(request.DurationMinutes),
                DateTimeOffset.UtcNow,
                request.StaffMemberId);

            var response = new AppointmentResponse(
                appointment.Id,
                appointment.SalonId,
                appointment.CustomerId,
                appointment.ServiceId,
                appointment.StaffMemberId,
                appointment.StartsAtUtc,
                appointment.EndsAtUtc,
                (int)appointment.Duration.TotalMinutes,
                appointment.Status.ToString());

            return Created(
                $"/api/appointments/{appointment.Id}",
                response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { Error = exception.Message });
        }
    }
}

public sealed record ScheduleAppointmentRequest(
    Guid SalonId,
    Guid CustomerId,
    Guid ServiceId,
    Guid? StaffMemberId,
    DateTimeOffset StartsAtUtc,
    int DurationMinutes);

public sealed record AppointmentResponse(
    Guid Id,
    Guid SalonId,
    Guid CustomerId,
    Guid ServiceId,
    Guid? StaffMemberId,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    int DurationMinutes,
    string Status);
