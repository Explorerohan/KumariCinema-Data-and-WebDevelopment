using Microsoft.AspNetCore.Mvc;
using CinemaTicketing.Data;

namespace CinemaTicketing.Controllers;

/// <summary>
/// Complex page: User Ticket - tickets bought by user in last 6 months
/// </summary>
public class UserTicketController : Controller
{
    private readonly CustomerRepository _customerRepo;
    private readonly ReportRepository _reportRepo;

    public UserTicketController(CustomerRepository customerRepo, ReportRepository reportRepo)
    {
        _customerRepo = customerRepo;
        _reportRepo = reportRepo;
    }

    public IActionResult Index(decimal? customerId)
    {
        ViewBag.Customers = _customerRepo.GetAll()
            .Select(c => new { c.CustomerId, Display = $"{c.FullName} ({c.Username})" })
            .ToList();

        if (!customerId.HasValue || customerId.Value == 0)
        {
            ViewBag.Tickets = new List<Models.ViewModels.UserTicketViewModel>();
            ViewBag.SelectedCustomer = null;
            return View();
        }

        var tickets = _reportRepo.GetUserTickets(customerId.Value);
        var customer = _customerRepo.GetById(customerId.Value);
        ViewBag.Tickets = tickets;
        ViewBag.SelectedCustomer = customer;
        ViewBag.CustomerId = customerId.Value;
        return View();
    }
}
