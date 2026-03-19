using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Models;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TheaterRepository _theaterRepo;
    private readonly MovieRepository _movieRepo;
    private readonly ShowingRepository _showingRepo;
    private readonly TicketRepository _ticketRepo;

    public HomeController(
        ILogger<HomeController> logger,
        TheaterRepository theaterRepo,
        MovieRepository movieRepo,
        ShowingRepository showingRepo,
        TicketRepository ticketRepo)
    {
        _logger = logger;
        _theaterRepo = theaterRepo;
        _movieRepo = movieRepo;
        _showingRepo = showingRepo;
        _ticketRepo = ticketRepo;
    }

    public IActionResult Index()
    {
        try
        {
            ViewBag.TheaterCount = _theaterRepo.GetAll().Count;
            ViewBag.MovieCount = _movieRepo.GetAll().Count;
            ViewBag.ShowingCount = _showingRepo.GetAll().Count;
            ViewBag.TicketCount = _ticketRepo.GetAll().Count;
        }
        catch
        {
            ViewBag.TheaterCount = null;
            ViewBag.MovieCount = null;
            ViewBag.ShowingCount = null;
            ViewBag.TicketCount = null;
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
