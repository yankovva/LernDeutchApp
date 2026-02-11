using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class VocabularyCardController(LerningAppContext dbcontext) :BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(string lessonId)
    {
        Guid lessonGuidId = Guid.Empty;
        if (!this.IsGuidValid(lessonId, ref lessonGuidId))
        {
            return this.RedirectToAction(nameof(this.Index));
        }
        
        var lesson = await dbcontext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonGuidId);

        if (lesson == null)
            return RedirectToAction("Index", "Lesson");

        var cards = await dbcontext.VocabularyCards
            .AsNoTracking()
            .Include(v => v.PartOfSpeech)
            .Include(v => v.Terms)
            .Where(v => v.LessonId == lessonGuidId)
            .Select(v => new VocabularyCardRowViewModel
            {
                Id = v.Id.ToString(),
                German = v.Terms.FirstOrDefault(t => t.Side == "de" && t.IsPrimary)!.Word,
                PartOfSpeech = v.PartOfSpeech.Name,
                Gender = v.Terms.FirstOrDefault(t => t.Side == "de" && t.IsPrimary)!.Gender ?? "-",
            })
            .ToListAsync();

        var model = new VocabularyCardsIndexViewModel
        {
            LessonId = lesson.Id.ToString(),
            LessonName = lesson.Name,
            Cards = cards
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        Guid cardId = Guid.Empty;
        if (!this.IsGuidValid(id, ref cardId))
        {
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