using CinemaDashboard.Data;
using CinemaDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Controllers;

public class CategoriesController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await db.Categories
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            var categories = await db.Categories.OrderBy(x => x.Name).ToListAsync();
            return View("Index", categories);
        }

        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories.FindAsync(id);

        if (category is null)
            return NotFound();

        if (await db.Movies.AnyAsync(x => x.CategoryId == id))
        {
            TempData["Error"] = "Delete or reassign the movies in this category first.";
            return RedirectToAction(nameof(Index));
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}