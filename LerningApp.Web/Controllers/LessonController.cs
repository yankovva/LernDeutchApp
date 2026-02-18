using System.Diagnostics;
using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.LessonSection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class LessonController(LerningAppContext dbcontext, 
    ILessonService lessonService,
    ICourseService courseService,
    UserManager<ApplicationUser> userManager) : BaseController
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
        var result = await lessonService.GetAddLessonToCourseByIdAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        
        return this.View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCourse(AddLessonToCourseViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }
        
        var result = await lessonService.AddLessonToCourseAsync(model);
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
    public async Task<IActionResult> Create()
    {
        AddLessonInputModel model = new AddLessonInputModel
        {
            Courses = await courseService.GetCourseOptionsAsync()
        };
        return this.View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(AddLessonInputModel model)
    {
        Guid userId = Guid.Parse(userManager.GetUserId(User)!);
        
        if (!this.ModelState.IsValid)
        {
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }

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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
       var result = await lessonService.GetLessonEditInputModelAsync(id);
       if (result.Result == false)
       {
           TempData["ErrorMessage"] = result.Message;
           return RedirectToAction(nameof(Index));
       }

       result.Data.Courses = await courseService.GetCourseOptionsAsync();

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

    var result = await lessonService.PostLessonEditInputModelAsync(model, id);
    if (result.Result == false)
    {
        ModelState.AddModelError(string.Empty, result.Message);
        model.Courses = await courseService.GetCourseOptionsAsync();
        return View(model);
    }
   
    
    TempData["SuccessMessage"] = $"Успешно редактирахте {model.Name}.";
    return RedirectToAction(nameof(Details), new { id });
}

}