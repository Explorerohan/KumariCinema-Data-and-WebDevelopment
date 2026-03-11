namespace CinemaTicketing.Models.ViewModels;

/// <summary>
/// View model for User Ticket report - tickets bought in last 6 months
/// </summary>
public class UserTicketViewModel
{
    public decimal? CustomerId { get; set; }
    public string? UserName { get; set; }
    public string? MovieTitle { get; set; }
    public string? TheaterName { get; set; }
    public string? HallNumber { get; set; }
    public DateTime? ShowDate { get; set; }
    public TimeSpan? ShowTime { get; set; }
    public string? TicketNumber { get; set; }
    public decimal? TicketPrice { get; set; }
    public string? PaymentStatus { get; set; }
}
