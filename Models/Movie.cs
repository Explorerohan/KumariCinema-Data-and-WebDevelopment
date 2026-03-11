using System.ComponentModel.DataAnnotations;

namespace CinemaTicketing.Models;

/// <summary>
/// Model for MOVIE table - movie details
/// </summary>
public class Movie
{
    public decimal MovieId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [Display(Name = "Title")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required")]
    [Display(Name = "Duration (minutes)")]
    [Range(1, 500, ErrorMessage = "Duration must be a number between 1 and 500 minutes")]
    public int Duration { get; set; }

    [Required(ErrorMessage = "Language is required")]
    [Display(Name = "Language")]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Language must contain only letters (no numbers allowed).")]
    public string? Language { get; set; }

    [Display(Name = "Genre")]
    [StringLength(100)]
    public string? Genre { get; set; }

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime? ReleaseDate { get; set; }
}
