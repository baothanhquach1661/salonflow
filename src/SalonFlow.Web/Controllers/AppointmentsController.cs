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

        return View(appointments);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var model = new CreateAppointmentViewModel
        {
            StartsAtLocal = DateTime.Now.AddHours(1)
        };

        await PopulateAppointmentOptionsAsync(
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
            await PopulateAppointmentOptionsAsync(
                model,
                cancellationToken);

            return View(model);
        }

        var selectedService = await _dbContext.SalonServices
            .AsNoTracking()
            .SingleOrDefaultAsync(
                service =>
                    service.Id == model.ServiceId!.Value &&
                    service.SalonId == DevelopmentSalonId &&
                    service.IsActive,
                cancellationToken);

        if (selectedService is null)
        {
            ModelState.AddModelError(
                nameof(model.ServiceId),
                "The selected service is not available.");

            await PopulateAppointmentOptionsAsync(
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

            await PopulateAppointmentOptionsAsync(
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
                selectedService.Id,
                new DateTimeOffset(localStart).ToUniversalTime(),
                selectedService.Duration,
                DateTimeOffset.UtcNow,
                selectedStaffMember?.Id);

            var details = new AppointmentDetails(
                appointment.Id,
                model.CustomerName,
                model.PhoneNumber,
                selectedService.Name,
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
            ModelState.AddModelError(
                string.Empty,
                exception.Message);

            await PopulateAppointmentOptionsAsync(
                model,
                cancellationToken);

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.CheckIn();

            await _dbContext.SaveChangesAsync(
                cancellationToken);

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
    public async Task<IActionResult> StartService(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.StartService();

            await _dbContext.SaveChangesAsync(
                cancellationToken);

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
    public async Task<IActionResult> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.Complete();

            await _dbContext.SaveChangesAsync(
                cancellationToken);

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
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (appointment is null)
        {
            return NotFound();
        }

        try
        {
            appointment.Cancel();

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            TempData["SuccessMessage"] =
                "Appointment cancelled successfully.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return RedirectToAction("Index", "Home");
    }

    private async Task PopulateAppointmentOptionsAsync(
        CreateAppointmentViewModel model,
        CancellationToken cancellationToken)
    {
        await PopulateAvailableServicesAsync(
            model,
            cancellationToken);

        await PopulateAvailableStaffMembersAsync(
            model,
            cancellationToken);
    }

    private async Task PopulateAvailableServicesAsync(
        CreateAppointmentViewModel model,
        CancellationToken cancellationToken)
    {
        var services = await _dbContext.SalonServices
            .AsNoTracking()
            .Where(service =>
                service.SalonId == DevelopmentSalonId &&
                service.IsActive)
            .OrderBy(service => service.Name)
            .ToListAsync(cancellationToken);

        model.AvailableServices = services
            .Select(service =>
                new SalonServiceOptionViewModel(
                    service.Id,
                    service.Name,
                    (int)service.Duration.TotalMinutes,
                    service.Price))
            .ToList();
    }

    private async Task PopulateAvailableStaffMembersAsync(
        CreateAppointmentViewModel model,
        CancellationToken cancellationToken)
    {
        model.AvailableStaffMembers =
            await _dbContext.StaffMembers
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
}
