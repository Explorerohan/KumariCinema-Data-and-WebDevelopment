using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for BOOKING table - supports ticket purchase logic
/// </summary>
public class Booking
{
    public decimal BookingId { get; set; }
    public decimal ShowingId { get; set; }
    public decimal CustomerId { get; set; }
    public DateTime? BookingDate { get; set; }
    public TimeSpan? BookingTime { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? BookingStatus { get; set; }
    public string? PaymentMethod { get; set; }
}
