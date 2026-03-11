using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// Complex page: TheaterCityHall Movie - movies and showtimes for theater and/or hall
/// </summary>
public class TheaterMovieController : Controller
{
    private readonly TheaterRepository _theaterRepo;
    private readonly HallRepository _hallRepo;
    private readonly ReportRepository _reportRepo;

    public TheaterMovieController(TheaterRepository theaterRepo, HallRepository hallRepo, ReportRepository reportRepo)
    {
        _theaterRepo = theaterRepo;
        _hallRepo = hallRepo;
        _reportRepo = reportRepo;
    }

    public IActionResult Index(decimal? theaterId, decimal? hallId)
    {
        ViewBag.Theaters = _theaterRepo.GetAll()
            .Select(t => new { t.TheaterId, Display = $"{t.TheaterName} ({t.City})" })
            .ToList();
        ViewBag.Halls = _hallRepo.GetAll()
            .Select(h => new { h.HallId, Display = $"{h.TheaterName} - Hall {h.HallNumber}" })
            .ToList();

        if (!theaterId.HasValue && !hallId.HasValue)
        {
            ViewBag.Results = new List<Models.ViewModels.TheaterMovieViewModel>();
            return View();
        }

        var results = _reportRepo.GetTheaterMovies(theaterId, hallId);
        ViewBag.Results = results;
        ViewBag.TheaterId = theaterId;
        ViewBag.HallId = hallId;
        return View();
    }
}
