using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for CUSTOMER table - user details
/// </summary>
public class Customer
{
    public decimal CustomerId { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    [Display(Name = "Full Name")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Address")]
    [StringLength(300)]
    public string? CustomerAddress { get; set; }

    [Display(Name = "City")]
    [StringLength(100)]
    public string? CustomerCity { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Registration Date")]
    [DataType(DataType.Date)]
    public DateTime? RegistrationDate { get; set; }
}
