using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class VocabularyCardController(LerningAppContext dbcontext,
    IVocabularyCardService vocabularyCardService) :BaseController
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
}