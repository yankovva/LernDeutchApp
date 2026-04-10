using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class VocabularyCardController(IVocabularyCardService vocabularyCardService,
    IPartOfSpeechService partOfSpeechService) :BaseController
{
    //DONE
    [HttpGet]
    public async Task<IActionResult> Index(string lessonId)
    {
        var userId = User.GetUserId()!;
        var result = await vocabularyCardService.IndexGetAllCardsForALessonAsync(lessonId, userId);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Lesson");
        }
        
        return View(result.Data);
    }
    //DONE
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var userId = User.GetUserId()!;
        var result = await vocabularyCardService.GetDetailsForACardAsync(id, userId);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return this.RedirectToAction(nameof(this.Index));
        }
        
        return this.View(result.Data);
    }
    //Done
    [HttpGet]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Create(string lessonId)
    {
        var userId = User.GetUserId()!;
        var result = await vocabularyCardService.GetCreateVocabularyCardAsync(lessonId, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return this.RedirectToAction("Index", "Lesson");
        }
        return this.View(result.Data);
    }
    
    //Done
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Create(VocabularyCardCreateInputModel model)
    {
        if (!ModelState.IsValid)
        {
            model.PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync();
            return View(model);
        }
        var userId = User.GetUserId();
        var result = await vocabularyCardService.CreateVocabularyCardAsync(model, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            model.PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync();
            return View(model);
        }
        
        TempData["SuccessMessage"] = "Успешно създадохте нова карта";
        return RedirectToAction(nameof(Index), new { lessonId = model.LessonId });
    }
    //Done
    [HttpGet]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Edit(string id)
    {
        var userId = User.GetUserId();
        var result = await vocabularyCardService.GetCardEditByIdAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return this.RedirectToAction("Index", "Home");
        }
        result.Data!.PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync();
        
        return this.View(result.Data);
    }
    //Done
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Edit(VocabularyCardEditInputModel model,string id)
    {
        var userId = User.GetUserId()!;
        var result = await vocabularyCardService.PostCardEditByIdAsync(model,id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return this.RedirectToAction("Index", "Home");
        }
        return RedirectToAction(nameof(Index), new { lessonId = model.LessonId });
    }
    //Done
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Delete(string id, string lessonId)
    {
        var userId = User.GetUserId();
        var result = await vocabularyCardService.DeleteCardByIdAsync(id,userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return this.RedirectToAction("Index", "Home");
        }
        TempData["SuccessMessage"] = "Успешно премахнахте картата.";
        return RedirectToAction(nameof(Index), new {lessonId });
    }
    
    //Done
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> SoftDelete(string id, string lessonId)
    {
        var userId = User.GetUserId()!;
        var result = await vocabularyCardService.SoftDeleteCardAsync(id, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return this.RedirectToAction("Index", "Home");
        }
        
        TempData["SuccessMessage"] = $"Успешно изтрихте картата";
        return RedirectToAction(nameof(Index), new {lessonId });
    }
}