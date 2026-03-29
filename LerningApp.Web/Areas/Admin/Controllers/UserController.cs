using LerningApp.Services.Data.Interfaces.AdminInterfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.ApplicationConstants;


namespace LerningApp.Areas.Admin.Controllers;

[Area(AdminRole)]
[Authorize(Roles = AdminRole)]
public class UserController(IAdminUserService userService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await userService.GetAllUsersNotDeletedAsync();
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string userId)
    {
        var result = await userService.DeleteUserAsync(userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        
        TempData["SuccessMessage"] = "User deleted.";
        return RedirectToAction(nameof(Index));
    }
}