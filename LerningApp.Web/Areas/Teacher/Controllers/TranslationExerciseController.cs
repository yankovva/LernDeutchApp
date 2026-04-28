using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.TranslationExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Areas.Teacher.Controllers;

[Authorize(Roles = "Admin, Teacher")]
[Area("Teacher")]
public class TranslationExerciseController(ITranslationExerciseService translationExerciseService) : Controller
{
    //Done
    [HttpGet]
    public async Task<IActionResult> Create(string lessonId)
    {
        var userId = User.GetUserId();
        var result = await translationExerciseService.GetAddTranslationExercisesAsync(lessonId, userId!);
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
    public async Task<IActionResult> Create(CreateTranslationExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var userId = User.GetUserId()!;
        var result = await translationExerciseService.AddTranslationExerciseAsync(model, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index), "Home");
        }
        
        TempData["SuccessMessage"] = "Успешно създадохте упражнението";
        return RedirectToAction(nameof(Create), new { lessonId = model.LessonId });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(string id, string lessonId)
    {
        var userId = User.GetUserId()!;
        var result = await translationExerciseService
            .SoftDeleteAsync(id, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return View(id);
        }
        
        TempData["SuccessMessage"] = "Successfully deleted exercise.";
        return RedirectToAction("Manage", "Lesson", new { area = "Teacher", id = lessonId });
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        string userId = User.GetUserId()!;
        var result = await translationExerciseService.GetEditTranslation(id, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        return View(result.Data);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditTranslationExerciseViewModel model)
    {
        string userId = User.GetUserId()!;
        var result = await translationExerciseService.PostEditranslation(model, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Manage", "Lesson", new { area = "Teacher", id = model.LessonId });
    }
    
    
}