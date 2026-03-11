using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for SHOWING table - showtimes details
/// </summary>
public class Showing
{
    public decimal ShowingId { get; set; }

    [Required(ErrorMessage = "Hall is required")]
    [Display(Name = "Hall")]
    public decimal HallId { get; set; }

    [Required(ErrorMessage = "Movie is required")]
    [Display(Name = "Movie")]
    public decimal MovieId { get; set; }

    [Required(ErrorMessage = "Show date is required")]
    [Display(Name = "Show Date")]
    [DataType(DataType.Date)]
    public DateTime ShowDate { get; set; }

    [Required(ErrorMessage = "Show time is required")]
    [Display(Name = "Show Time")]
    public TimeSpan ShowTime { get; set; }

    [Display(Name = "Status")]
    [StringLength(20)]
    public string? Status { get; set; }

    // Navigation display
    public string? HallNumber { get; set; }
    public string? MovieTitle { get; set; }
    public string? TheaterName { get; set; }
}
