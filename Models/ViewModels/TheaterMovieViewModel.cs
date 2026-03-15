namespace CinemaTicketing.Models.ViewModels;

/// <summary>
/// View model for TheaterCityHall Movie report
/// </summary>
public class TheaterMovieViewModel
{
    public string? TheaterName { get; set; }
    public string? City { get; set; }
    public string? HallNumber { get; set; }
    public string? MovieTitle { get; set; }
    public string? Genre { get; set; }
    public string? Language { get; set; }
    public DateTime? ShowDate { get; set; }
    public string? ShowTime { get; set; }
    public string? Status { get; set; }
}
