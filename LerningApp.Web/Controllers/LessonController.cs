using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class LessonController(ILessonService lessonService,
    ICourseService courseService,
    ITeacherService teacherService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    { 
        var lessons = await lessonService.IndexGetLessonsAsync();
        return View(lessons);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var result = await  lessonService.GetLessonDetailsAsync(id);
        if (result.Result == false )
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        
        return this.View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> AddToCourse(string id)
    {
        string userId = User.GetUserId()!;
        
        var result = await lessonService.GetAddLessonToCourseByIdAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        
        return this.View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
            if (result.Field != null)
                ModelState.AddModelError(string.Empty, result.Message);
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create()
    {
        string userId = User.GetUserId()!;
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (!isTeacher)
        {
            TempData["ErrorMessage"] = "Нямате права";
            return RedirectToAction(nameof(Index));
        }
        AddLessonInputModel model = new AddLessonInputModel
        {
            Courses = await courseService.GetCourseOptionsAsync()
        };
        return this.View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddLessonInputModel model)
    {
        if (!this.ModelState.IsValid)
        {
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }
        
        string userId = User.GetUserId()!;
        var result = await  lessonService.AddLessonAsync(model, userId);
        if (result.Result == false)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
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
           return RedirectToAction(nameof(Index));
       }

       result.Data!.Courses = await courseService.GetCourseOptionsAsync();

       return View(result.Data);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LessonEditInputModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }
        string userId = User.GetUserId()!;
        var result = await lessonService.PostLessonEditInputModelAsync(model, id, userId);
        if (result.Result == false)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }
       
        
        TempData["SuccessMessage"] = $"Успешно редактирахте {model.Name}.";
        return RedirectToAction(nameof(Details), new { id });
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
            return RedirectToAction("Details", new { id = id });
        }
        
        TempData["SuccessMessage"] = $"Успешно изтрихте урока";
        return RedirectToAction(nameof(Index));
    }
}