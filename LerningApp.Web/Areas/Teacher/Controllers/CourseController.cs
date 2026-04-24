using LerningApp.Common;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Areas.Teacher.Controllers;

[Area(TeacherRole)]
[Authorize(Roles = "Admin,Teacher")]
public class CourseController(ILevelService levelService,
    ICourseService courseService) : Controller
{
   
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var courses = await courseService.GetAllCorsesAsync();
        return View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> Manage(string id)
    {
        var model = await courseService
            .GetCourseManageByIdAsync(id);
        return View(model.Data);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        string userId = User.GetUserId()!;
       
        AddCourseViewModel model = new AddCourseViewModel
        {
            Levels =  await levelService.GetAllLevelOptionsAsync()
        };

        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
            if (result.ErrorType == Enums.ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            
            model.Levels = await levelService.GetAllLevelOptionsAsync();
            return this.View(model);
        }

        TempData["SuccessMessage"] = "Успешно създадохте курс.";
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
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
    
    [HttpPost]
    [ValidateAntiForgeryToken]
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
            if (result.ErrorType == Enums.ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            
            model.Levels = await levelService.GetAllLevelOptionsAsync();
            return this.View(model);
        }

        TempData["SuccessMessage"] = "Успешно редактирахте курса.";
        return RedirectToAction("Details", "Course", new { area = TeacherRole, id = id });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
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
    
    [HttpPost]
    [ValidateAntiForgeryToken]
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
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(string id)
    {
        string userId = User.GetUserId()!;
        
        var result = await courseService.SoftDeleteCourseAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        
        TempData["SuccessMessage"] = "Successfully deleted.";
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(string id)
    {
        string userId = User.GetUserId()!;
        var result = await courseService.PublishCourseAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
       
        TempData["SuccessMessage"] = "Successfully published.";
        return RedirectToAction(nameof(Index));
    }
}