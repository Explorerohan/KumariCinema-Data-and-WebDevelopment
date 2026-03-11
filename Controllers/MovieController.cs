using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Models;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// CRUD controller for MOVIE (Movie Details)
/// </summary>
public class MovieController : Controller
{
    private readonly MovieRepository _repo;

    public MovieController(MovieRepository repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        var list = _repo.GetAll();
        return View(list);
    }

    public IActionResult Create()
    {
        return View(new Movie());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Movie model)
    {
        FixDurationValidationMessage();
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Insert(model);
                TempData["Success"] = "Movie created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
        return View(model);
    }

    public IActionResult Edit(decimal id)
    {
        var item = _repo.GetById(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Movie model)
    {
        FixDurationValidationMessage();
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Update(model);
                TempData["Success"] = "Movie updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
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
            TempData["Success"] = "Movie deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Replace generic Duration format error with meaningful message when user enters non-digits
    /// </summary>
    private void FixDurationValidationMessage()
    {
        if (ModelState.TryGetValue("Duration", out var entry) && entry?.Errors.Count > 0)
        {
            var msg = entry.Errors[0].ErrorMessage;
            if (msg.Contains("is not valid") || msg.Contains("not valid for"))
            {
                entry.Errors.Clear();
                entry.Errors.Add("Duration must contain only digits (numbers). Enter a value between 1 and 500.");
            }
        }
    }
}
