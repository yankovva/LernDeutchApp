using LerningApp.Data;
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

}