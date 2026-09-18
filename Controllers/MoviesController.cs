using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.Services;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Controllers;

public class MoviesController(AppDbContext db, IImageService images) : Controller
{
    public async Task<IActionResult> Index()
    {
        var movies = await db.Movies
            .Include(x => x.Category)
            .Include(x => x.Cinema)
            .Include(x => x.MovieActors)
            .ThenInclude(x => x.Actor)
            .OrderByDescending(x => x.ShowDateTime)
            .ToListAsync();

        return View(movies);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateLists();
        return View(new MovieFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLists();
            return View(form);
        }

        var movie = new Movie
        {
            Name = form.Name,
            Description = form.Description,
            Price = form.Price,
            IsAvailable = form.IsAvailable,
            ShowDateTime = form.ShowDateTime,
            CategoryId = form.CategoryId,
            CinemaId = form.CinemaId,
            MainImageUrl = await images.SaveAsync(form.MainImage, "movies")
        };

        foreach (var file in form.SubImages)
        {
            var url = await images.SaveAsync(file, "movies");
            if (url is not null)
            {
                movie.Images.Add(new MovieImage { Url = url });
            }
        }

        movie.MovieActors = form.ActorIds
            .Distinct()
            .Select(id => new MovieActor { ActorId = id })
            .ToList();

        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var movie = await db.Movies
            .Include(x => x.Images)
            .Include(x => x.MovieActors)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (movie is null)
            return NotFound();

        await PopulateLists();

        return View(new MovieFormViewModel
        {
            Id = movie.Id,
            Name = movie.Name,
            Description = movie.Description,
            Price = movie.Price,
            IsAvailable = movie.IsAvailable,
            ShowDateTime = movie.ShowDateTime,
            CategoryId = movie.CategoryId,
            CinemaId = movie.CinemaId,
            ExistingMainImageUrl = movie.MainImageUrl,
            ExistingImages = movie.Images.ToList(),
            ActorIds = movie.MovieActors.Select(x => x.ActorId).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieFormViewModel form)
    {
        var movie = await db.Movies
            .Include(x => x.Images)
            .Include(x => x.MovieActors)
            .FirstOrDefaultAsync(x => x.Id == form.Id);

        if (movie is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            form.ExistingMainImageUrl = movie.MainImageUrl;
            form.ExistingImages = movie.Images.ToList();
            await PopulateLists();
            return View(form);
        }

        movie.Name = form.Name;
        movie.Description = form.Description;
        movie.Price = form.Price;
        movie.IsAvailable = form.IsAvailable;
        movie.ShowDateTime = form.ShowDateTime;
        movie.CategoryId = form.CategoryId;
        movie.CinemaId = form.CinemaId;

        if (form.MainImage is not null)
        {
            var url = await images.SaveAsync(form.MainImage, "movies");
            images.Delete(movie.MainImageUrl);
            movie.MainImageUrl = url;
        }

        foreach (var file in form.SubImages)
        {
            var url = await images.SaveAsync(file, "movies");
            if (url is not null)
            {
                movie.Images.Add(new MovieImage { Url = url });
            }
        }

        db.RemoveRange(movie.MovieActors);
        movie.MovieActors = form.ActorIds
            .Distinct()
            .Select(id => new MovieActor { MovieId = movie.Id, ActorId = id })
            .ToList();

        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id, int movieId)
    {
        var image = await db.MovieImages.FindAsync(id);

        if (image is not null)
        {
            images.Delete(image.Url);
            db.MovieImages.Remove(image);
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Edit), new { id = movieId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await db.Movies
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (movie is null)
            return NotFound();

        images.Delete(movie.MainImageUrl);

        foreach (var image in movie.Images)
        {
            images.Delete(image.Url);
        }

        db.Movies.Remove(movie);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLists()
    {
        ViewBag.Categories = new SelectList(
            await db.Categories.OrderBy(x => x.Name).ToListAsync(),
            "Id",
            "Name"
        );

        ViewBag.Cinemas = new SelectList(
            await db.Cinemas.OrderBy(x => x.Name).ToListAsync(),
            "Id",
            "Name"
        );

        ViewBag.Actors = await db.Actors.OrderBy(x => x.Name).ToListAsync();
    }
}