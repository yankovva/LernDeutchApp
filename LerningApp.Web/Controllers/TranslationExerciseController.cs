using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.TranslationExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class TranslationExerciseController(ITranslationExerciseService translationExerciseService) : Controller
{
    //Done
    [HttpGet]
    [Authorize(Roles = "Admin, Teacher")]
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
    [Authorize(Roles = "Admin, Teacher")]
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
}