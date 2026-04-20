using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.Enums;

namespace LerningApp.Controllers;

public class CourseController(ICourseService courseService,
    ILevelService levelService,
    ITeacherService teacherService) : BaseController
{
    //Done
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId =  User.GetUserId();
       
        IEnumerable<CourseIndexViewModel> courses = await courseService
            .IndexGetCoursesAsync(userId);

        return this.View(courses);
    }

    //Done
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        string? userId = User.GetUserId();

        var result = await courseService.GetCourseDetailsByIdAsync(id, userId);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = $"{result.Message}";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }
    //Done
    [Authorize]
    [HttpGet]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Create()
    {
        string userId = User.GetUserId()!;
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);

        if (!isTeacher)
        {
            TempData["ErrorMessage"] = AccessDeniedMessage ;
            return RedirectToAction(nameof(Index));
        }
       
        AddCourseViewModel model = new AddCourseViewModel
        {
            Levels =  await levelService.GetAllLevelOptionsAsync()
        };

        return this.View(model);
    }

    //Done
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Create(AddCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = await levelService.GetAllLevelOptionsAsync();
            return this.View(model);
        }
        
        string userId = User.GetUserId()!;
        var result = await courseService.AddCourseAsync(model, userId);
        
        if (result.Result == false)
        {
            if (result.ErrorType == ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            
            model.Levels = await levelService.GetAllLevelOptionsAsync();
            return this.View(model);
        }

        TempData["SuccessMessage"] = "Успешно създадохте курс.";
        return RedirectToAction(nameof(Index));
    }

    //Done
    [Authorize]
    [HttpGet]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Edit(string id)
    {
        string userId = User.GetUserId()!;
        
        var result = await courseService.GetCourseEditByIdAsync(id, userId);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = $"{result.Message}";
            return RedirectToAction(nameof(Index));
        }

        result.Data!.Levels = await levelService.GetAllLevelOptionsAsync();
        return View(result.Data);
    }
    
    //Done
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Edit(CourseEditViewModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = await levelService.GetAllLevelOptionsAsync();
            return this.View(model);
        }
        
        string userId = User.GetUserId()!;
       
        var result = await courseService.PostEditCourseAsync(model, id, userId);
        if (result.Result == false)
        {
            if (result.ErrorType == ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            
            model.Levels = await levelService.GetAllLevelOptionsAsync();
            return this.View(model);
        }

        TempData["SuccessMessage"] = "Успешно редактирахте курса.";
        return RedirectToAction(nameof(Details), new { Id = id });
    }
    
    //Done
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Deactivate(string id)
    {
        string userId = User.GetUserId()!;
        var result = await courseService.DeactivateCourseAsync(id,userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Успешно деактивирахте курса.";
        return RedirectToAction(nameof(Index));
    }

    //Done
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Restore(string id)
    {
        string userId = User.GetUserId()!;
      
        var result = await courseService.RestoreCourseAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Успешно активирахте курса.";
        return RedirectToAction(nameof(Index));
    }
    
    //Done
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(string courseId)
    {
        Guid userId = Guid.Parse(User.GetUserId()!);
        var result = await courseService.EnrollInCourseAsync(courseId, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = $"Успешно се запизахте за курса.";
        return RedirectToAction("Details", new { id = courseId });
    }
    
    //Done
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> SoftDelete(string id)
    {
        string userId = User.GetUserId()!;
        
        var result = await courseService.SoftDeleteCourseAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Details", new { id = id });
        }
        
        TempData["SuccessMessage"] = $"Успешно изтрихте курса";
        return RedirectToAction(nameof(Index));
    }
}
