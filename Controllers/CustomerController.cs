using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Models;
using CinemaTicketing.Data;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Controllers;

/// <summary>
/// CRUD controller for CUSTOMER (User Details)
/// </summary>
public class CustomerController : Controller
{
    private readonly CustomerRepository _repo;

    public CustomerController(CustomerRepository repo)
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
        return View(new Customer { RegistrationDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer model)
    {
        if (_repo.UsernameExists(model.Username))
            ModelState.AddModelError("Username", "Username already exists.");

        if (ModelState.IsValid)
        {
            try
            {
                _repo.Insert(model);
                TempData["Success"] = "Customer created successfully.";
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
    public IActionResult Edit(Customer model)
    {
        if (_repo.UsernameExists(model.Username, model.CustomerId))
            ModelState.AddModelError("Username", "Username already exists.");

        if (ModelState.IsValid)
        {
            try
            {
                _repo.Update(model);
                TempData["Success"] = "Customer updated successfully.";
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
            TempData["Success"] = "Customer deleted successfully.";
        }
        catch (OracleException ex) when (ex.Number == 2292)
        {
            TempData["Error"] = "Cannot delete: This customer has related bookings or tickets. Delete their bookings/tickets first, or enable cascade delete in the database.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
