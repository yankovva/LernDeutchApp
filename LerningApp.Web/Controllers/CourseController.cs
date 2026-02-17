using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Level;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class CourseController(LerningAppContext dbcontext,
    UserManager<ApplicationUser> userManager,
    ICourseService courseService,
    ILevelService levelService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User);
        Guid? userGuidId = Guid.TryParse(userId, out var parsed)
            ? parsed
            : null;

        IEnumerable<CourseIndexViewModel> courses = await courseService
            .IndexGetCoursesAsync(userGuidId);
       
        return this.View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        string? userId = userManager.GetUserId(User);
        
        var result = await courseService.GetCourseDetailsByIdAsync(id, userId);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = $"{result.Message}";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        AddCourseViewModel model = new AddCourseViewModel
        {
            Levels = await GetAllLevelsFromDbAsync()
        };

        return this.View(model);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(AddCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = await levelService.GetAllLevelsFromDbAsLevelOptionsAsync();
            return this.View(model);
        }

        var result = await courseService.AddCourseAsync(model);
        if (result.Result == false)
        {
            if (string.IsNullOrWhiteSpace(result.Field))
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                ModelState.AddModelError(result.Field ?? "", result.Message!);
            }
            model.Levels = await levelService.GetAllLevelsFromDbAsLevelOptionsAsync();
            return this.View(model);
        }     
        
        TempData["SuccessMessage"] = "Успешно създадохте курс.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var result = await  courseService.GetCourseEditByIdAsync(id);
     
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = $"{result.Message}";
            return RedirectToAction(nameof(Index));
        }
        
        result.Data.Levels = await levelService.GetAllLevelsFromDbAsLevelOptionsAsync();
        return View(result.Data);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CourseEditViewModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = await GetAllLevelsFromDbAsync();
            return this.View(model);
        }

        var result = await courseService.PostEditCourseAsync(model, id);
        if (result.Result == false)
        {
            if (string.IsNullOrWhiteSpace(result.Field))
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                ModelState.AddModelError(result.Field ?? "", result.Message!);
            }
            model.Levels = await GetAllLevelsFromDbAsync();
            return this.View(model);
        }

        TempData["SuccessMessage"] = "Успешно редактирахте курса.";
        return RedirectToAction(nameof(Details), new { Id = id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Deactivate(string id)
    {
       var result = await courseService.DeactivateCourseAsync(id);
       if (result.Result == false)
       {
           TempData["ErrorMessage"] = result.Message;
           return RedirectToAction(nameof(Index));
       }
       
       TempData["SuccessMessage"] = "Успешно деактивирахте курса.";
       return RedirectToAction(nameof(Index));
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Restore(string id)
    {
       var result = await courseService.RestoreCourseAsync(id);
       if (result.Result == false)
       {
           TempData["ErrorMessage"] = result.Message;
           return RedirectToAction(nameof(Index));
       }
        
        TempData["SuccessMessage"] = "Успешно активирахте курса.";
        return RedirectToAction(nameof(Index));
    }
  
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Enroll(string courseId)
    { 
        var userId = Guid.Parse(userManager.GetUserId(User)!);
        
        Guid courseGuidId = Guid.Empty;
        if (!IsGuidValid(courseId, ref courseGuidId))
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }

        Course? course = await dbcontext
            .Courses
            .FirstOrDefaultAsync(c => c.Id == courseGuidId && c.IsPublished == true);

        if (course == null)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }
        
        bool alreadyEnrolled = await dbcontext.UsersCourses
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseGuidId);

        if (alreadyEnrolled)
        {
            TempData["InfoMessage"] = "Вече сте записани за този курс.";
            return RedirectToAction("Details", new { id = courseId });
        }

        UserCourse newUserCourse = new UserCourse
        {
            UserId = userId,
            CourseId = courseGuidId,
            StartedAt = DateTime.UtcNow
        };

        await dbcontext.UsersCourses.AddAsync(newUserCourse);
        await dbcontext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = $"Успешно се запизахте за курс {course.Name}.";
        return RedirectToAction("Details", new { id = courseId });
    }
    

private async Task<List<LevelOptionsViewModel>> GetAllLevelsFromDbAsync()
    {
        var levels = await dbcontext.Levels
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new LevelOptionsViewModel
            {
                Id = c.Id.ToString(), 
                Name = c.Name
            })
            .ToListAsync();
        
        return levels;
    }
}