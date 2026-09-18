using CinemaDashboard.Models;
using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.ViewModels;

public class MovieFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime ShowDateTime { get; set; } = DateTime.Now.AddDays(1);

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int CinemaId { get; set; }

    public IFormFile? MainImage { get; set; }

    public List<IFormFile> SubImages { get; set; } = [];

    public List<int> ActorIds { get; set; } = [];

    public string? ExistingMainImageUrl { get; set; }

    public List<MovieImage> ExistingImages { get; set; } = [];
}