using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Web.ViewModels.TranslationExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class TranslationExerciseController(LerningAppContext dbContext,
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

        TranslationExercise exercise = new TranslationExercise()
        {
            LessonId = Guid.Parse(model.LessonId),
            GermanSentence = model.GermanCorrectTranslation,
            EnglishSentence = model.SentenceEn,
            BulgarianSentence = model.SentenceBg,
            OrderIndex = model.OrderIndex,
            PublisherId = currentUserId
        };
       
        await dbContext.TranslationExercises.AddAsync(exercise);
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Успешно създадохте упражнението";
        return RedirectToAction(nameof(Create), new { lessonId = model.LessonId });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckTranslationExercise(string exerciseId, string lessonId, string userAnswer)
    {
        if (!Guid.TryParse(exerciseId, out var exId))
        {
            return Json(new { isCorrect = false });
        }

        var exercise = await exerciseRepository
            .GetByIdAsync(exId);

        if (exercise == null)
        {
            return Json(new { isCorrect = false });
        }

        bool isCorrect = string.Equals(userAnswer?.Trim(), exercise.GermanSentence.Trim(),
            StringComparison.OrdinalIgnoreCase);

        return Json(new { isCorrect, correctAnswer = exercise.GermanSentence });
    }

}