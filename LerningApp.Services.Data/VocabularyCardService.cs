using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LerningApp.Services.Data;

public class VocabularyCardService(IRepository<VocabularyCard,Guid> vocabularyCardRepository,
    IRepository<Lesson,Guid> lessonRepository): IVocabularyCardService
{
    public async Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuidId))
        {
            return ServiceResultT<VocabularyCardsIndexViewModel>.Fail("Урокът не е намерен.");
        }
        
        var lesson = await lessonRepository
            .GetByIdAsync(lessonGuidId);
          
        if (lesson == null)
        {
            return ServiceResultT<VocabularyCardsIndexViewModel>.Fail("Урокът не е намерен.");
        }

        var cards = await vocabularyCardRepository
            .GetAllAttached()
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
            LessonId = lessonId,
            LessonName = lesson.Name,
            Cards = cards
        };
        
        return ServiceResultT<VocabularyCardsIndexViewModel>.Success(model);
    }

    public async Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid cardGuid))
        {
            return ServiceResultT<VocabularyCardDetailsViewModel>.Fail("Картата не е намерена."); 
            // return this.RedirectToAction(nameof(this.Index));
        }

        VocabularyCard? card = await vocabularyCardRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(vc => vc.Lesson)
            .Include(vc => vc.PartOfSpeech)
            .Include(vc => vc.Terms)
            .FirstOrDefaultAsync(vc => vc.Id == cardGuid);

        if (card == null)
        {
            return ServiceResultT<VocabularyCardDetailsViewModel>.Fail("Картата не е намерена.");
           // return this.RedirectToAction(nameof(this.Index));
        }

        VocabularyCardDetailsViewModel model = new VocabularyCardDetailsViewModel
        {
            Id = card.Id.ToString(),
            LessonId = card.LessonId.ToString(),
            LessonName = card.Lesson?.Name ?? "Урок",
            ImageUrl = card.ImagePath!,
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
        
        return ServiceResultT<VocabularyCardDetailsViewModel>.Success(model);
    }
}