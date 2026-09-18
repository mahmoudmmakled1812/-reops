using CinemaDashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN},{RoleConstants.EMPLOYEE}")]
public class ActorController : Controller
{
    //private readonly ApplicationDbContext _db = new();
    private readonly IRepository<Actor> _repository;// = new Repository<Actor>();

    public ActorController(IRepository<Actor> repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string? query, int page = 1, int size = 4)
    {
        var Actors = _repository.Get();

        if (query is not null)
            Actors = Actors.Where(e => e.Name.ToLower().Contains(query.ToLower()));

        // ViewBag Vs ViewData (MVC only)
        //ViewBag.Query = query ?? "";
        //ViewData["Query"] = query ?? "";

        var totalPages = Math.Ceiling(Actors.Count() / (double)size);
        Actors = Actors.Skip((page - 1) * size).Take(size);

        return View(new ActorWithFilterVM
        {
            Actors = Actors,
            Query = query ?? "",
            TotalPages = totalPages,
            CurrentPage = page,
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Actor Actor, IFormFile Img, CancellationToken ct = default) // photo.png
    {
        if (Img is not null && Img.Length > 0)
        {
            //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
            //var fileName = Img.FileName + DateTime.Now.ToString("dd-MM-yyyy") + Path.GetExtension(Img.FileName);
            var fileName = $"{Guid.NewGuid().ToString()}-{DateTime.Now.ToString("dd-MM-yyyy")}{Path.GetExtension(Img.FileName)}";

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "Actors", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }

            Actor.Logo = fileName;
        }

        //_db.Actors.Add(new Actor()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});

        await _repository.CreateAsync(Actor, ct);
        await _repository.CommitAsync(ct);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Create Actor Successfully";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var Actor = _repository.GetOne(e => e.Id == id, tracked: false);

        if (Actor is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        return View(Actor);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Actor Actor, IFormFile Img, CancellationToken ct = default)
    {
        var ActorInDB = _repository.GetOne(e => e.Id == Actor.Id, tracked: false);

        if (ActorInDB is null) return NotFound();

        if (Img is not null && Img.Length > 0)
        {
            // Save New Img in wwwroot

            //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
            //var fileName = Img.FileName + DateTime.Now.ToString("dd-MM-yyyy") + Path.GetExtension(Img.FileName);
            var fileName = $"{Guid.NewGuid().ToString()}-{DateTime.Now.ToString("dd-MM-yyyy")}{Path.GetExtension(Img.FileName)}";

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "Actors", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }

            // Delete Old Img from wwwroot

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "Actors", ActorInDB.Logo);

            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            // Replace img in DB

            Actor.Logo = fileName;
        }
        else
            Actor.Logo = ActorInDB.Logo;

        //_db.Actors.Add(new Actor()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});
        _repository.Update(Actor);
        await _repository.CommitAsync(ct);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Update Actor Successfully";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var Actor = _repository.GetOne(e => e.Id == id);

        if (Actor is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        _repository.Delete(Actor);
        await _repository.CommitAsync(ct);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Delete Actor Successfully";

        return RedirectToAction(nameof(Index));
    }
}