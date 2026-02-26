using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.TranslationExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class TranslationExerciseController(LerningAppContext dbContext, UserManager<ApplicationUser> userManager) : Controller
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
}