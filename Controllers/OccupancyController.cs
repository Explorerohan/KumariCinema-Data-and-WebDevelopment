using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// Complex page: MovieTheatherCityHallOccupancy - top 3 theater/hall by occupancy
/// Only paid tickets count for occupancy calculation.
/// </summary>
public class OccupancyController : Controller
{
    private readonly MovieRepository _movieRepo;
    private readonly ReportRepository _reportRepo;

    public OccupancyController(MovieRepository movieRepo, ReportRepository reportRepo)
    {
        _movieRepo = movieRepo;
        _reportRepo = reportRepo;
    }

    public IActionResult Index(decimal? movieId)
    {
        ViewBag.Movies = _movieRepo.GetAll()
            .Select(m => new { m.MovieId, Display = m.Title })
            .ToList();

        if (!movieId.HasValue || movieId.Value == 0)
        {
            ViewBag.Results = new List<Models.ViewModels.OccupancyViewModel>();
            ViewBag.SelectedMovie = null;
            return View();
        }

        var results = _reportRepo.GetTopOccupancy(movieId.Value);
        var movie = _movieRepo.GetById(movieId.Value);
        ViewBag.Results = results;
        ViewBag.SelectedMovie = movie;
        ViewBag.MovieId = movieId.Value;
        return View();
    }
}
