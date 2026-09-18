using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Controllers;

public class ActorsController(AppDbContext db, IImageService images) : Controller
{
    public async Task<IActionResult> Index()
    {
        var actors = await db.Actors
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View(actors);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Actor actor, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await db.Actors.ToListAsync());
        }

        actor.ImageUrl = await images.SaveAsync(image, "actors");
        db.Actors.Add(actor);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = await db.Actors.FindAsync(id);

        if (actor is null)
            return NotFound();

        images.Delete(actor.ImageUrl);
        db.Actors.Remove(actor);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}