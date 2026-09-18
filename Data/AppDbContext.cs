using CinemaDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Cinema> Cinemas => Set<Cinema>();
    public DbSet<Actor> Actors => Set<Actor>();

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieImage> MovieImages => Set<MovieImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieActor>().HasKey(x => new { x.MovieId, x.ActorId });
        modelBuilder.Entity<Movie>().Property(x => x.Price).HasColumnType("decimal(18,2)");
        base.OnModelCreating(modelBuilder);
    }
}