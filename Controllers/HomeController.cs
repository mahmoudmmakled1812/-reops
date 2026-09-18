using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CinemaDashboard.Controllers;

public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        int movies = await db.Movies.CountAsync();
        var dashboard = new DashboardViewModel(
      await db.Categories.CountAsync(),
      await db.Cinemas.CountAsync(),
      movies,
      await db.Actors.CountAsync(),
      await db.Movies.CountAsync(x => x.IsAvailable)
       );

        return View("Dashboard", dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}