using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalonFlow.Infrastructure.Persistence;
using SalonFlow.Web.Models;

namespace SalonFlow.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly SalonFlowDbContext _dbContext;

    public HomeController(SalonFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var appointments = await _dbContext.AppointmentDetails
            .AsNoTracking()
            .Include(item => item.Appointment)
            .OrderBy(item => item.Appointment.StartsAtUtc)
            .ToListAsync();

        var today = DateTime.Today;

        var todaysAppointments = appointments
            .Where(item =>
                item.Appointment.StartsAtUtc
                    .ToLocalTime()
                    .Date == today)
            .ToList();

        return View(
            new DashboardViewModel(todaysAppointments));
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
