using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class VocabularyCardController(IVocabularyCardService vocabularyCardService,
    IPartOfSpeechService partOfSpeechService) :BaseController
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
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var result = await vocabularyCardService.GetCardEditByIdAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index), new { lessonId = result.Data?.LessonId });
        }
        result.Data!.PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync();
        
        return this.View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(VocabularyCardEditInputModel model,string id)
    {
        var result = await vocabularyCardService.PostCardEditByIdAsync(model,id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index), new { lessonId = model.LessonId });
        }
        
        return RedirectToAction(nameof(Index), new { lessonId = model.LessonId });
    }
    [HttpPost]
    public async Task<IActionResult> Delete(string id, string lessonId)
    {
        var result = await vocabularyCardService.DeleteCardByIdAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index), new { lessonId });
        }
        TempData["SuccessMessage"] = "Успешно премахнахте картата.";
        return RedirectToAction(nameof(Index), new {lessonId });
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SoftDelete(string id)
    {
        var result = await vocabularyCardService.SoftDeleteCardAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Details", new { id = id });
        }
        
        TempData["SuccessMessage"] = $"Успешно изтрихте картата";
        return RedirectToAction(nameof(Index));
    }
}