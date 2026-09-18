using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Schema;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN}")]
public class UserController : Controller
{
    // 1. get all user from userManager.Users

    public IActionResult GetAllUsers()
    {
        return View();
    }

    // 2. filter
    [HttpPost]
    public IActionResult FilterUsers(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction(nameof(GetAllUsers));
        }

        return View();
    }

    // 3. pagination
    [HttpGet]
    public IActionResult PaginateUsers(int page, int size)
    {
        if (page < 1 || size < 1)
        {
            return RedirectToAction(nameof(GetAllUsers));
        }
        return View();
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!User.IsInRole(RoleConstants.SUPER_ADMIN))
        {
            return Forbid();
        }
        return View();
    }

    [HttpPost]
    public IActionResult ChangeRole(string userId, string newRole)
    {
        return View();
    }
}