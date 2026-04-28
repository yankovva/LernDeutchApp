using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.ListeningExercise;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
    
using static LerningApp.Common.Enums;

namespace LerningApp.Areas.Teacher.Controllers;

[Authorize(Roles = "Admin, Teacher")]
[Area("Teacher")]
public class ListeningExerciseController(IListeningExerciseService exerciseService) : Controller
{
    //Done
    [HttpGet]
    public async Task<IActionResult> Create(string lessonId)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.CreateGetListeningExercise(lessonId, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return View(result.Data);
    }
    //Done
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateListeningExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        string userId = User.GetUserId()!;
        
        var result = await exerciseService.CreatePostListeningExercise(model, userId);
        if (result.Result == false)
        {
            if (result.ErrorType == ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            return this.View(model);
        }

        return RedirectToAction("Manage", "Lesson", new {area = "Teacher", id = model.LessonId });
    }
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.GetEditListeningExercise(id, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        return View(result.Data);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditListeningExerciseViewModel model)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.PostEditListeningExercise(model, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return View(model);
        }
        return RedirectToAction("Manage", "Lesson", new { area = "Teacher", id = model.LessonId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(string id , string lessonId)
    {
        var userId = User.GetUserId()!;
        var result = await exerciseService
            .SoftDeleteExerciseAsync(id, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return View(id);
        }
        
        TempData["SuccessMessage"] = "Successfully deleted exercise.";
        return RedirectToAction("Manage", "Lesson", new { area = "Teacher", id = lessonId });
    }
}
