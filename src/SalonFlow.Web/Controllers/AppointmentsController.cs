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
    public async Task<IActionResult> Create(
    CancellationToken cancellationToken)
    {
        var model = new CreateAppointmentViewModel
        {
            StartsAtLocal = DateTime.Now.AddHours(1),
            DurationMinutes = 45
        };

        await PopulateAvailableStaffMembersAsync(
            model,
            cancellationToken);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    CreateAppointmentViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAvailableStaffMembersAsync(
                model,
                cancellationToken);

            return View(model);
        }

        var selectedStaffMember = model.StaffMemberId.HasValue
            ? await _dbContext.StaffMembers
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    staffMember =>
                        staffMember.Id == model.StaffMemberId.Value &&
                        staffMember.SalonId == DevelopmentSalonId &&
                        staffMember.IsActive,
                    cancellationToken)
            : null;

        if (model.StaffMemberId.HasValue &&
            selectedStaffMember is null)
        {
            ModelState.AddModelError(
                nameof(model.StaffMemberId),
                "The selected staff member is not available.");

            await PopulateAvailableStaffMembersAsync(
                model,
                cancellationToken);

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
                selectedStaffMember?.Id);

            var details = new AppointmentDetails(
                appointment.Id,
                model.CustomerName,
                model.PhoneNumber,
                model.ServiceName,
                selectedStaffMember?.Name,
                model.Notes);

            _dbContext.Appointments.Add(appointment);
            _dbContext.AppointmentDetails.Add(details);

            await _dbContext.SaveChangesAsync(cancellationToken);

            TempData["SuccessMessage"] =
                "Appointment created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            await PopulateAvailableStaffMembersAsync(
                model,
                cancellationToken);

            return View(model);
        }
    }

    private async Task PopulateAvailableStaffMembersAsync(
    CreateAppointmentViewModel model,
    CancellationToken cancellationToken)
    {
        model.AvailableStaffMembers = await _dbContext.StaffMembers
            .AsNoTracking()
            .Where(staffMember =>
                staffMember.SalonId == DevelopmentSalonId &&
                staffMember.IsActive)
            .OrderBy(staffMember => staffMember.Name)
            .Select(staffMember =>
                new StaffMemberOptionViewModel(
                    staffMember.Id,
                    staffMember.Name))
            .ToListAsync(cancellationToken);
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
