using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for THEATER table - theater details
/// </summary>
public class Theater
{
    public decimal TheaterId { get; set; }

    [Required(ErrorMessage = "Theater name is required")]
    [Display(Name = "Theater Name")]
    [StringLength(150)]
    public string TheaterName { get; set; } = string.Empty;

    [Display(Name = "City")]
    [StringLength(100)]
    public string? City { get; set; }

    [Display(Name = "Address")]
    [StringLength(300)]
    public string? Address { get; set; }

    [Display(Name = "Contact Number")]
    [StringLength(20)]
    public string? ContactNumber { get; set; }

    [Display(Name = "Email")]
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }
}
