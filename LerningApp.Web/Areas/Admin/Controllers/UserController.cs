using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Areas.Admin.Controllers;

public class UserController(IAdminUserService userService) : Controller
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var model = await userService.GetAllUsersAsync();
        return View(model);
    }
}