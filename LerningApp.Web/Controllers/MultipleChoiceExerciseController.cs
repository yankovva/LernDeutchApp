using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

[Authorize]
public class MultipleChoiceExerciseController(UserManager<ApplicationUser> userManager,
    IMultipleChoiceExerciseService exerciseService ) : Controller
{
    
    [HttpGet]
    public IActionResult Create(string lessonId)
    {
        var model = new CreateMultipleChoiceExerciseViewModel()
        {
          LessonId = lessonId
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMultipleChoiceExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        string currentUserId = userManager.GetUserId(User)!;
        var result = await exerciseService.CreateAsync(model, currentUserId);
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