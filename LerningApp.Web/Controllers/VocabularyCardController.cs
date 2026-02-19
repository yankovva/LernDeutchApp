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
        Guid cardId = Guid.Empty;
        if (!this.IsGuidValid(id, ref cardId))
        {
            TempData["ErrorMessage"] = "Картата не е намерена.";
            return this.RedirectToAction(nameof(this.Index));
        }

        VocabularyCard? card = await dbcontext
            .VocabularyCards
            .AsNoTracking()
            .Include(vc => vc.Lesson)
            .Include(vc => vc.PartOfSpeech)
            .Include(vc => vc.Terms)
            .FirstOrDefaultAsync(vc => vc.Id == cardId);

        if (card == null)
        {
            TempData["ErrorMessage"] = "Картата не е намерена.";
            return this.RedirectToAction(nameof(this.Index));
        }

        VocabularyCardDetailsViewModel model = new VocabularyCardDetailsViewModel
        {
            Id = card.Id.ToString(),
            LessonId = card.LessonId.ToString(),
            LessonName = card.Lesson?.Name ?? "Урок",
            GermanWord = card.Terms.FirstOrDefault(t => t.Side == "de" && t.IsPrimary)?.Word ?? "",
            BulgarianTranslation = card.Terms.FirstOrDefault(t => t.Side == "bg" && t.IsPrimary)?.Word,
            EnglishTranslation = card.Terms.FirstOrDefault(t => t.Side == "en" && t.IsPrimary)?.Word,
            PartOfSpeech = card.PartOfSpeech.Name,
            BulgarianSynonyms = card.Terms.Where(t => t.Side == "bg" && !t.IsPrimary)
                .Select(t => t.Word)
                .ToList(),
            EnglishSynonyms = card.Terms
                .Where(t => t.Side == "en" && !t.IsPrimary)
                .Select(t => t.Word)
                .ToList(),
            Gender = card.Terms.FirstOrDefault(t => t.Side == "de" && t.IsPrimary)?.Gender ?? "-",
            ExampleSentence = card.Terms.FirstOrDefault(t => t.Side == "de" && t.IsPrimary)?.ExampleSentence ?? "-",
        };
        
        return this.View(model);
    }
}