using System.ComponentModel.DataAnnotations;

namespace SalonFlow.Web.Models;

public sealed class CreateAppointmentViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(30)]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Service")]
    public string ServiceName { get; set; } = string.Empty;

    [Display(Name = "Staff Member")]
    public Guid? StaffMemberId { get; set; }

    public IReadOnlyList<StaffMemberOptionViewModel>
        AvailableStaffMembers
    { get; set; } = [];

    [Required]
    [Display(Name = "Start Time")]
    public DateTime? StartsAtLocal { get; set; }

    [Range(15, 480)]
    [Display(Name = "Duration")]
    public int DurationMinutes { get; set; } = 45;

    [StringLength(500)]
    public string? Notes { get; set; }

    public Guid? CreatedAppointmentId { get; set; }

    public string? CreatedStatus { get; set; }

    public DateTimeOffset? StartsAtUtc { get; set; }

    public DateTimeOffset? EndsAtUtc { get; set; }
}

public sealed record StaffMemberOptionViewModel(
    Guid Id,
    string Name);
