namespace CinemaDashboard.Models;

public class MovieImage
{
    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public int MovieId { get; set; }

    public Movie Movie { get; set; } = null!;
}