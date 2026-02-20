using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class VocabularyCardController(IVocabularyCardService vocabularyCardService,
    IPartOfSpeechService partOfSpeechService,
    LerningAppContext dbContext ) :BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(string lessonId)
    {
        var result = await vocabularyCardService.IndexGetAllCardsForALessonAsync(lessonId);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Lesson");
        }
        
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var result = await vocabularyCardService.GetDetailsForACardAsync(id);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return this.RedirectToAction(nameof(this.Index));
        }
        
        return this.View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(string lessonId)
    {
        var model = new VocabularyCardCreateInputModel()
        {
            LessonId = lessonId,
            PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync()
        };

        return this.View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(VocabularyCardCreateInputModel model)
    {
        if (!ModelState.IsValid)
        {
            model.PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync();
            return View(model);
        }

        var result = await vocabularyCardService.CreateVocabularyCardAsync(model);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            model.PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync();
            return View(model);
        }
        
        TempData["SuccessMessage"] = "Успешно създадохте нова карта";
        return RedirectToAction(nameof(Index), new { lessonId = model.LessonId });
    }
}