using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.TranslationExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class TranslationExerciseController(ITranslationExerciseService translationExerciseService,
    UserManager<ApplicationUser> userManager,
    IRepository<TranslationExercise, Guid> exerciseRepository) : Controller
{
    [HttpGet]
    public  IActionResult Create(string lessonId)
    {
        var model = new CreateTranslationExerciseViewModel()
        {
            LessonId = lessonId
        };
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTranslationExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var currentUserId = Guid.Parse(userManager.GetUserId(User)!);

        var result = await translationExerciseService.AddTranslationExerciseAsync(model, currentUserId);
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
    public async Task<IActionResult> CheckTranslationExercise(string exerciseId, string userAnswer)
    {
        var result = await translationExerciseService
            .CheckTranslationAsync(exerciseId, userAnswer);

        if (result == null)
        {
            return Json(new { isCorrect = false });
        }

        return Json(new { result.Value.isCorrect, result.Value.correctAnswer });
    }

}