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
        if (!Guid.TryParse(lessonId, out var id))
            return RedirectToAction("Index", "Lesson");

        var lesson = await dbcontext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
            return RedirectToAction("Index", "Lesson");

        var cards = await dbcontext.VocabularyCards
            .AsNoTracking()
            .Include(v => v.PartOfSpeech)
            .Include(v => v.Terms)
            .Where(v => v.LessonId == id)
            .Select(v => new VocabularyCardRowViewModel
            {
                Id = v.Id.ToString(),
                German = v.Terms.FirstOrDefault(t => t.Side == "de" && t.IsPrimary)!.Text,
                Bulgarian = v.Terms.FirstOrDefault(t => t.Side == "bg" && t.IsPrimary)!.Text,
                English = v.Terms.FirstOrDefault(t => t.Side == "en" && t.IsPrimary)!.Text,
                PartOfSpeech = v.PartOfSpeech.Name
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
            GermanWord = card.Terms.FirstOrDefault(t => t.Side == "de" && t.IsPrimary)?.Text ?? "",
            BulgarianTranslation = card.Terms.FirstOrDefault(t => t.Side == "bg" && t.IsPrimary)?.Text,
            EnglishTranslation = card.Terms.FirstOrDefault(t => t.Side == "en" && t.IsPrimary)?.Text,
            PartOfSpeech = card.PartOfSpeech.Name,
            BulgarianSynonyms = card.Terms.Where(t => t.Side == "bg" && !t.IsPrimary)
                .Select(t => t.Text)
                .ToList(),
            EnglishSynonyms = card.Terms
                .Where(t => t.Side == "en" && !t.IsPrimary)
                .Select(t => t.Text)
                .ToList(),
        };
        return this.View(model);
        
    }
}