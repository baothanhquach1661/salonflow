using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalonFlow.Infrastructure.Persistence;
using SalonFlow.Web.Models;

namespace SalonFlow.Web.Controllers;

public sealed class HomeController : Controller
{
    private static readonly Guid DevelopmentSalonId =
    Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly SalonFlowDbContext _dbContext;

    public HomeController(SalonFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index(
    CancellationToken cancellationToken)
    {
        var appointments = await _dbContext.AppointmentDetails
            .AsNoTracking()
            .Include(item => item.Appointment)
            .Where(item =>
                item.Appointment.SalonId == DevelopmentSalonId)
            .OrderBy(item => item.Appointment.StartsAtUtc)
            .ToListAsync(cancellationToken);

        var today = DateTime.Today;

        var todaysAppointments = appointments
            .Where(item =>
                item.Appointment.StartsAtUtc
                    .ToLocalTime()
                    .Date == today)
            .ToList();

        var availableStaffCount = await _dbContext.StaffMembers
            .AsNoTracking()
            .CountAsync(
                staffMember =>
                    staffMember.SalonId == DevelopmentSalonId &&
                    staffMember.IsActive,
                cancellationToken);

        return View(new DashboardViewModel(
            todaysAppointments,
            availableStaffCount));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id
                ?? HttpContext.TraceIdentifier
        });
    }
}
