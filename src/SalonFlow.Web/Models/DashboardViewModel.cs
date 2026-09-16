using SalonFlow.Domain.Appointments;
using SalonFlow.Infrastructure.Persistence.Appointments;

namespace SalonFlow.Web.Models;

public sealed record DashboardViewModel(
    IReadOnlyList<AppointmentDetails> TodaysAppointments)
{
    public int TodaysAppointmentCount =>
        TodaysAppointments.Count;

    public int WaitingCount =>
        TodaysAppointments.Count(item =>
            item.Appointment.Status == AppointmentStatus.CheckedIn);

    public int InServiceCount =>
        TodaysAppointments.Count(item =>
            item.Appointment.Status == AppointmentStatus.InService);
}
