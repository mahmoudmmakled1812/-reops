using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class Category
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Movie> Movies { get; set; } = [];
}