using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class Actor
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Bio { get; set; }

    public string? ImageUrl { get; set; }
    public ICollection<MovieActor> MovieActors { get; set; } = [];
}