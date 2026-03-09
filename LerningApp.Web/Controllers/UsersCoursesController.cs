using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class UsersCoursesController(IUsersCoursesService usersCoursesService, 
    UserManager<ApplicationUser> userManager)
    : BaseController
{
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {  
        var userId =  userManager.GetUserId(User)!;
        var result = await usersCoursesService.IndexGetAllUsersCoursesAsync(userId);
        if (result.Result == false)
        {
           TempData["ErrorMessage"] = result.Message;
           return RedirectToAction("Index", "Course");
        }
        
        return View(result.Data);
    }
}