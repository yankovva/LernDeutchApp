using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.ListeningExercise;
using static LerningApp.Common.EntityErrorMessages.Exercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class ListeningExerciseController(IListeningExerciseService exerciseService) :BaseController
{
    [HttpGet]
    public async Task<IActionResult> Create(string lessonId)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.CreateGetListeningExercise(userId, lessonId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return View(result.Data);
    }

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
            if (result.Field != null)
                ModelState.AddModelError(string.Empty, result.Message);
            else
                TempData["ErrorMessage"] = result.Message;
            return  RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Details", "Lesson", new { id = model.LessonId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckListeningExercise(string questionId, string lessonId, string selectedAnswer)
    {
        string userId = User.GetUserId()!;
        
        var result = await exerciseService
            .CheckListeningExerciseAnswer(questionId, selectedAnswer,lessonId, userId);

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