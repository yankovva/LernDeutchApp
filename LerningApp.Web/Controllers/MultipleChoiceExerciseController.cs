using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LerningApp.Controllers;

[Authorize]
public class MultipleChoiceExerciseController(IMultipleChoiceExerciseService exerciseService) : Controller
{
    
    [HttpGet]
    public async Task<IActionResult> Create(string lessonId)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.GetCreateAsync(lessonId, userId!);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMultipleChoiceExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        string userId = User.GetUserId()!;
        var result = await exerciseService.CreateAsync(model, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return View(model);
        }
       
        TempData["SuccessMessage"] = "Успешно създадохте упражнението";
        return RedirectToAction(nameof(Create), new { lessonId = model.LessonId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckMultipleChoice(string exerciseId, string lessonId, string selectedAnswer)
    {
        var result = await exerciseService
            .CheckMultipleChoice(exerciseId, selectedAnswer);

        if (result == null)
        {
            TempData["ErrorMessage"] = "Невалидно упражнение.";
            return RedirectToAction("Details", "Lesson" ,new { id = lessonId });
        }
        
        return Json(new
        {
            result.Value.isCorrect,
            result.Value.correctAnswer,
        });
    }
}