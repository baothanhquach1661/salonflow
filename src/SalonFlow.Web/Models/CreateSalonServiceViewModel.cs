using System.ComponentModel.DataAnnotations;

namespace SalonFlow.Web.Models;

public sealed class CreateSalonServiceViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Service Name")]
    public string Name { get; set; } = string.Empty;

    [Range(5, 480)]
    [Display(Name = "Duration")]
    public int DurationMinutes { get; set; } = 45;

    [Range(typeof(decimal), "0", "10000")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }
}
