namespace CinemaTicketing.Models.ViewModels;

/// <summary>
/// View model for MovieTheatherCityHallOccupancy report - top 3 by occupancy
/// </summary>
public class OccupancyViewModel
{
    public string? TheaterName { get; set; }
    public string? City { get; set; }
    public string? HallNumber { get; set; }
    public int Capacity { get; set; }
    public int PaidTicketsCount { get; set; }
    public decimal OccupancyPercentage { get; set; }
}
