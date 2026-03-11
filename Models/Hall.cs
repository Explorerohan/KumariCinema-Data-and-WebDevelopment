using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for HALL table - hall details (TheaterCityHall via THEATER + HALL join)
/// </summary>
public class Hall
{
    public decimal HallId { get; set; }

    [Required(ErrorMessage = "Theater is required")]
    [Display(Name = "Theater")]
    public decimal TheaterId { get; set; }

    [Required(ErrorMessage = "Hall number is required")]
    [Display(Name = "Hall Number")]
    [StringLength(20)]
    public string HallNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Capacity is required")]
    [Display(Name = "Capacity")]
    [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
    public int Capacity { get; set; }

    [Display(Name = "Hall Type")]
    [StringLength(50)]
    public string? HallType { get; set; }

    // Navigation - joined display
    public string? TheaterName { get; set; }
    public string? City { get; set; }
}
