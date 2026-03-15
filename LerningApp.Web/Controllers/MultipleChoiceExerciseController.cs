using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LerningApp.Controllers;

[Authorize]
public class MultipleChoiceExerciseController(IMultipleChoiceExerciseService exerciseService,
    ITeacherService teacherService,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository) : Controller
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
        string userId = User.GetUserId()!;
       
        var result = await exerciseService
            .CheckMultipleChoice(exerciseId, selectedAnswer,lessonId, userId);

        if (result == null)
        {
            return BadRequest(new { message = "Invalid operation." });
        }
        
        return Json(new
        {
            result.Value.isCorrect,
            result.Value.correctAnswer,
        });
    }
}