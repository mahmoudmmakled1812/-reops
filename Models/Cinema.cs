using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class Cinema
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public ICollection<Movie> Movies { get; set; } = [];
}