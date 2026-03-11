using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Models;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// CRUD controller for HALL (TheaterCityHall - THEATER + HALL join)
/// </summary>
public class HallController : Controller
{
    private readonly HallRepository _repo;
    private readonly TheaterRepository _theaterRepo;

    public HallController(HallRepository repo, TheaterRepository theaterRepo)
    {
        _repo = repo;
        _theaterRepo = theaterRepo;
    }

    public IActionResult Index()
    {
        var list = _repo.GetAll();
        return View(list);
    }

    public IActionResult Create()
    {
        ViewBag.Theaters = _theaterRepo.GetAll()
            .Select(t => new { t.TheaterId, Display = $"{t.TheaterName} ({t.City})" })
            .ToList();
        return View(new Hall());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Hall model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Insert(model);
                TempData["Success"] = "Hall created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
        ViewBag.Theaters = _theaterRepo.GetAll()
            .Select(t => new { t.TheaterId, Display = $"{t.TheaterName} ({t.City})" })
            .ToList();
        return View(model);
    }

    public IActionResult Edit(decimal id)
    {
        var item = _repo.GetById(id);
        if (item == null) return NotFound();
        ViewBag.Theaters = _theaterRepo.GetAll()
            .Select(t => new { t.TheaterId, Display = $"{t.TheaterName} ({t.City})" })
            .ToList();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Hall model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Update(model);
                TempData["Success"] = "Hall updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
        ViewBag.Theaters = _theaterRepo.GetAll()
            .Select(t => new { t.TheaterId, Display = $"{t.TheaterName} ({t.City})" })
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
            TempData["Success"] = "Hall deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
