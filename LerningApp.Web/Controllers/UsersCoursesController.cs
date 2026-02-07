using System.Security.Claims;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.UsersCouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

[Authorize]
public class UsersCoursesController(LerningAppContext dbcontext, UserManager<ApplicationUser> userManager)
    : BaseController
{
 
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
         var userId =  userManager.GetUserId(User)!;

         List<MyCourseCardViewModel> courses =  await dbcontext.UsersCourses
             .AsNoTracking()
             .Include(uc => uc.Course)
             .ThenInclude(c => c.Level)
             .Where(uc => uc.UserId.ToString() == userId)
             .Select(uc => new MyCourseCardViewModel
             {
                 Id = uc.Course.Id.ToString(),
                 Name = uc.Course.Name,
                 Description = uc.Course.Description,
                 LevelName = uc.Course.Level.Name,
                 StartedAt = uc.StartedAt,
                 CompletedAt = uc.CompletedAt
             })
             .ToListAsync();
         
         var model = new UsersCoursesIndexViewModel { Courses = courses };
         
        return View(model);
    }
}