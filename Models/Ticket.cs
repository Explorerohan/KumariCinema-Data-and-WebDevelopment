using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for TICKET table - ticket details
/// </summary>
public class Ticket
{
    public decimal TicketId { get; set; }

    [Required(ErrorMessage = "Booking is required")]
    [Display(Name = "Booking")]
    public decimal BookingId { get; set; }

    [Required(ErrorMessage = "Seat is required")]
    [Display(Name = "Seat")]
    public decimal SeatId { get; set; }

    [Required(ErrorMessage = "Ticket number is required")]
    [Display(Name = "Ticket Number")]
    [StringLength(50)]
    public string TicketNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ticket price is required")]
    [Display(Name = "Ticket Price")]
    [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10000")]
    public decimal TicketPrice { get; set; }

    [Display(Name = "Ticket Status")]
    [StringLength(20)]
    public string? TicketStatus { get; set; }

    [Display(Name = "Issue Date")]
    [DataType(DataType.Date)]
    public DateTime? IssueDate { get; set; }

    // Navigation display
    public string? MovieTitle { get; set; }
    public string? TheaterName { get; set; }
    public string? HallNumber { get; set; }
    public DateTime? ShowDate { get; set; }
    public string? ShowTime { get; set; }
    public string? PaymentStatus { get; set; }
}
