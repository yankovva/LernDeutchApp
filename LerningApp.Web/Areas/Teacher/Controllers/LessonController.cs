using LerningApp.Common;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Lesson;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Areas.Teacher.Controllers;

[Area(TeacherRole)]
[Authorize(Roles = "Admin,Teacher")]
public class LessonController(ILessonService lessonService,
    ICourseService courseService) : Controller
{
   [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lessons = await lessonService.GetAllLessonsAsync();
        return View(lessons);
    }
    
    [HttpGet]
    public async Task<IActionResult> AddToCourse(string id)
    {
        string userId = User.GetUserId()!;
        
        var result = await lessonService.GetAddLessonToCourseByIdAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return this.View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> AddToCourse(AddLessonToCourseViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }
        string userId = User.GetUserId()!;
        
        var result = await lessonService.AddLessonToCourseAsync(model, userId);
        if (result.Result == false)
        {
            if (result.ErrorType == Enums.ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            
            return this.View(model);
        }
        
        // TODO: consider enum for result action
        if (string.IsNullOrWhiteSpace(model.SelectedCourseId))
            TempData["SuccessMessage"] = "Урокът беше премахнат от курса.";
        else
            TempData["SuccessMessage"] = "Урокът беше добавен към курса.";
        
        return RedirectToAction(nameof(this.Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        string userId = User.GetUserId()!;
        AddLessonInputModel model = new AddLessonInputModel
        {
            Courses = await courseService.GetAssignableCourseOptionsAsync()
        };
        return this.View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddLessonInputModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Courses = await courseService.GetAssignableCourseOptionsAsync();
            return View(model);
        }
        
        string userId = User.GetUserId()!;
        var result = await  lessonService.AddLessonAsync(model, userId);
        if (result.Result == false)
        {
            if (result.ErrorType == Enums.ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            model.Courses = await courseService.GetAssignableCourseOptionsAsync();
            return this.View(model);
        }
        
        TempData["SuccessMessage"] = $"Успешно създадохте {model.Name}.";
        return this.RedirectToAction(nameof(this.Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        string userId = User.GetUserId()!;
       var result = await lessonService.GetLessonEditInputModelAsync(id, userId);
       if (result.Result == false)
       {
           TempData["ErrorMessage"] = result.Message;
           return RedirectToAction("Index", "Home");
       }

       result.Data!.Courses = await courseService.GetAssignableCourseOptionsAsync();

       return View(result.Data);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LessonEditInputModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            model.Courses = await courseService.GetAssignableCourseOptionsAsync();
            return View(model);
        }
        string userId = User.GetUserId()!;
        var result = await lessonService.PostLessonEditInputModelAsync(model, id, userId);
        if (result.Result == false)
        {
            if (result.ErrorType == Enums.ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            model.Courses = await courseService.GetAssignableCourseOptionsAsync();
            return this.View(model);
        }
       
        TempData["SuccessMessage"] = $"Успешно редактирахте {model.Name}.";
        return RedirectToAction(nameof(Details), "Lesson", new { id });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(string id)
    {
        string userId = User.GetUserId()!;
        var result = await lessonService.SoftDeleteLessonAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        TempData["SuccessMessage"] = $"Успешно изтрихте урока";
        return RedirectToAction(nameof(Index));
    }
}