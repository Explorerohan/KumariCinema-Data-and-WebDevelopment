using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// Read-only listing controller for THEATER table.
/// Theater details are managed directly in the database for this academic demo.
/// </summary>
public class TheaterController : Controller
{
    private readonly TheaterRepository _repo;

    public TheaterController(TheaterRepository repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        var theaters = _repo.GetAll();
        return View(theaters);
    }
}

