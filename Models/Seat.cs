using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for SEAT table - seat details
/// </summary>
public class Seat
{
    public decimal SeatId { get; set; }

    [Required(ErrorMessage = "Hall is required")]
    [Display(Name = "Hall")]
    public decimal HallId { get; set; }

    [Required(ErrorMessage = "Seat number is required")]
    [Display(Name = "Seat Number")]
    [StringLength(20)]
    public string SeatNumber { get; set; } = string.Empty;

    [Display(Name = "Row Number")]
    [StringLength(10)]
    public string? RowNumber { get; set; }

    [Display(Name = "Seat Type")]
    [StringLength(20)]
    public string? SeatType { get; set; }
}
