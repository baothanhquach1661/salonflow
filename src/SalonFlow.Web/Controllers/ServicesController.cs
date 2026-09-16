using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalonFlow.Domain.Services;
using SalonFlow.Infrastructure.Persistence;
using SalonFlow.Web.Models;

namespace SalonFlow.Web.Controllers;

public sealed class ServicesController : Controller
{
    private static readonly Guid DevelopmentSalonId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly SalonFlowDbContext _dbContext;

    public ServicesController(SalonFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var services = await _dbContext.SalonServices
            .AsNoTracking()
            .Where(service =>
                service.SalonId == DevelopmentSalonId)
            .OrderBy(service => service.Name)
            .ToListAsync(cancellationToken);

        return View(services);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateSalonServiceViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateSalonServiceViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var serviceName = model.Name.Trim();

        var alreadyExists = await _dbContext.SalonServices
            .AnyAsync(
                service =>
                    service.SalonId == DevelopmentSalonId &&
                    service.Name == serviceName,
                cancellationToken);

        if (alreadyExists)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "A service with this name already exists.");

            return View(model);
        }

        try
        {
            var service = SalonService.Create(
                DevelopmentSalonId,
                serviceName,
                TimeSpan.FromMinutes(model.DurationMinutes),
                model.Price);

            _dbContext.SalonServices.Add(service);

            await _dbContext.SaveChangesAsync(cancellationToken);

            TempData["SuccessMessage"] =
                "Service created successfully.";

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
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var service = await _dbContext.SalonServices
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (service is null)
        {
            return NotFound();
        }

        service.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] =
            $"{service.Name} has been deactivated.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var service = await _dbContext.SalonServices
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (service is null)
        {
            return NotFound();
        }

        service.Activate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] =
            $"{service.Name} has been activated.";

        return RedirectToAction(nameof(Index));
    }
}
