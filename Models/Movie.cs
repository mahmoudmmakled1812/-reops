using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class Movie
{
    public int Id { get; set; }

    [Required, StringLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(3000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Display(Name = "Show time")]
    public DateTime ShowDateTime { get; set; }

    public string? MainImageUrl { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int CinemaId { get; set; }

    public Category? Category { get; set; }

    public Cinema? Cinema { get; set; }

    public ICollection<MovieImage> Images { get; set; } = [];

    public ICollection<MovieActor> MovieActors { get; set; } = [];
}