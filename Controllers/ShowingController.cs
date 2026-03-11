using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Models;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// CRUD controller for SHOWING (Showtimes Details)
/// </summary>
public class ShowingController : Controller
{
    private readonly ShowingRepository _repo;
    private readonly HallRepository _hallRepo;
    private readonly MovieRepository _movieRepo;

    public ShowingController(ShowingRepository repo, HallRepository hallRepo, MovieRepository movieRepo)
    {
        _repo = repo;
        _hallRepo = hallRepo;
        _movieRepo = movieRepo;
    }

    public IActionResult Index()
    {
        var list = _repo.GetAll();
        return View(list);
    }

    public IActionResult Create()
    {
        ViewBag.Halls = _hallRepo.GetAll()
            .Select(h => new { h.HallId, Display = $"{h.TheaterName} - Hall {h.HallNumber}" })
            .ToList();
        ViewBag.Movies = _movieRepo.GetAll()
            .Select(m => new { m.MovieId, Display = m.Title })
            .ToList();
        return View(new Showing { ShowDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Showing model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Insert(model);
                TempData["Success"] = "Showing created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
        ViewBag.Halls = _hallRepo.GetAll()
            .Select(h => new { h.HallId, Display = $"{h.TheaterName} - Hall {h.HallNumber}" })
            .ToList();
        ViewBag.Movies = _movieRepo.GetAll()
            .Select(m => new { m.MovieId, Display = m.Title })
            .ToList();
        return View(model);
    }

    public IActionResult Edit(decimal id)
    {
        var item = _repo.GetById(id);
        if (item == null) return NotFound();
        ViewBag.Halls = _hallRepo.GetAll()
            .Select(h => new { h.HallId, Display = $"{h.TheaterName} - Hall {h.HallNumber}" })
            .ToList();
        ViewBag.Movies = _movieRepo.GetAll()
            .Select(m => new { m.MovieId, Display = m.Title })
            .ToList();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Showing model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Update(model);
                TempData["Success"] = "Showing updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
        ViewBag.Halls = _hallRepo.GetAll()
            .Select(h => new { h.HallId, Display = $"{h.TheaterName} - Hall {h.HallNumber}" })
            .ToList();
        ViewBag.Movies = _movieRepo.GetAll()
            .Select(m => new { m.MovieId, Display = m.Title })
            .ToList();
        return View(model);
    }

    public IActionResult Delete(decimal id)
    {
        var item = _repo.GetById(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(decimal id)
    {
        try
        {
            _repo.Delete(id);
            TempData["Success"] = "Showing deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
