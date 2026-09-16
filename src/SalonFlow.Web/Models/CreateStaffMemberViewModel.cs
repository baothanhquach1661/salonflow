using System.ComponentModel.DataAnnotations;

namespace SalonFlow.Web.Models;

public sealed class CreateStaffMemberViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Phone]
    [StringLength(30)]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }
}
