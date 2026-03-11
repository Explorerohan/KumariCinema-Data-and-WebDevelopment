using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaTicketing.Models;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// CRUD controller for TICKET (Ticket Details with dropdowns)
/// </summary>
public class TicketController : Controller
{
    private readonly TicketRepository _repo;
    private readonly BookingRepository _bookingRepo;
    private readonly SeatRepository _seatRepo;

    public TicketController(TicketRepository repo, BookingRepository bookingRepo, SeatRepository seatRepo)
    {
        _repo = repo;
        _bookingRepo = bookingRepo;
        _seatRepo = seatRepo;
    }

    public IActionResult Index()
    {
        var list = _repo.GetAll();
        return View(list);
    }

    private void PopulateDropdowns()
    {
        var bookings = _bookingRepo.GetForDropdown();
        var seats = _seatRepo.GetForDropdown();
        ViewBag.Bookings = new SelectList(bookings.Select(b => new { Value = b.BookingId.ToString(), Text = b.Display }), "Value", "Text");
        ViewBag.Seats = new SelectList(seats.Select(s => new { Value = s.SeatId.ToString(), Text = s.Display }), "Value", "Text");
    }

    public IActionResult Create()
    {
        PopulateDropdowns();
        return View(new Ticket { IssueDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Ticket model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Insert(model);
                TempData["Success"] = "Ticket created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
        PopulateDropdowns();
        return View(model);
    }

    public IActionResult Edit(decimal id)
    {
        var item = _repo.GetById(id);
        if (item == null) return NotFound();
        PopulateDropdowns();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Ticket model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _repo.Update(model);
                TempData["Success"] = "Ticket updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
        }
        PopulateDropdowns();
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
            TempData["Success"] = "Ticket deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
