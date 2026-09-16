using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalonFlow.Domain.Appointments;
using SalonFlow.Infrastructure.Persistence;
using SalonFlow.Infrastructure.Persistence.Appointments;
using SalonFlow.Web.Models;

namespace SalonFlow.Web.Controllers;

public sealed class AppointmentsController : Controller
{
    private static readonly Guid DevelopmentSalonId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly SalonFlowDbContext _dbContext;

    public AppointmentsController(SalonFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var appointments = await _dbContext.AppointmentDetails
            .AsNoTracking()
            .Include(item => item.Appointment)
            .OrderBy(item => item.Appointment.StartsAtUtc)
            .ToListAsync();

        return View(appointments);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateAppointmentViewModel
        {
            StartsAtLocal = DateTime.Now.AddHours(1),
            DurationMinutes = 45
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateAppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var localStart = DateTime.SpecifyKind(
                model.StartsAtLocal!.Value,
                DateTimeKind.Local);

            var appointment = Appointment.Schedule(
                DevelopmentSalonId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateTimeOffset(localStart).ToUniversalTime(),
                TimeSpan.FromMinutes(model.DurationMinutes),
                DateTimeOffset.UtcNow,
                string.IsNullOrWhiteSpace(model.StaffMemberName)
                    ? null
                    : Guid.NewGuid());

            var details = new AppointmentDetails(
                appointment.Id,
                model.CustomerName,
                model.PhoneNumber,
                model.ServiceName,
                model.StaffMemberName,
                model.Notes);

            _dbContext.Appointments.Add(appointment);
            _dbContext.AppointmentDetails.Add(details);

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Appointment created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(Guid id)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(item => item.Id == id);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.CheckIn();
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Customer checked in successfully.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartService(Guid id)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(item => item.Id == id);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.StartService();
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Service started successfully.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(item => item.Id == id);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.Complete();
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Appointment completed successfully.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(item => item.Id == id);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.Cancel();
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Appointment cancelled successfully.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return RedirectToAction("Index", "Home");
    }
}
