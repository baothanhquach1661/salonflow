using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalonFlow.Domain.StaffMembers;
using SalonFlow.Infrastructure.Persistence;
using SalonFlow.Web.Models;

namespace SalonFlow.Web.Controllers;

public sealed class StaffMembersController : Controller
{
    private static readonly Guid DevelopmentSalonId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly SalonFlowDbContext _dbContext;

    public StaffMembersController(SalonFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var staffMembers = await _dbContext.StaffMembers
            .AsNoTracking()
            .Where(staffMember =>
                staffMember.SalonId == DevelopmentSalonId)
            .OrderBy(staffMember => staffMember.Name)
            .ToListAsync(cancellationToken);

        return View(staffMembers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateStaffMemberViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateStaffMemberViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var staffMember = StaffMember.Create(
                DevelopmentSalonId,
                model.Name,
                model.PhoneNumber,
                model.Email);

            _dbContext.StaffMembers.Add(staffMember);

            await _dbContext.SaveChangesAsync(cancellationToken);

            TempData["SuccessMessage"] =
                "Staff member created successfully.";

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
        var staffMember = await _dbContext.StaffMembers
            .SingleOrDefaultAsync(
                staffMember =>
                    staffMember.Id == id &&
                    staffMember.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (staffMember is null)
        {
            return NotFound();
        }

        staffMember.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] =
            $"{staffMember.Name} has been deactivated.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var staffMember = await _dbContext.StaffMembers
            .SingleOrDefaultAsync(
                staffMember =>
                    staffMember.Id == id &&
                    staffMember.SalonId == DevelopmentSalonId,
                cancellationToken);

        if (staffMember is null)
        {
            return NotFound();
        }

        staffMember.Activate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] =
            $"{staffMember.Name} has been activated.";

        return RedirectToAction(nameof(Index));
    }
}
