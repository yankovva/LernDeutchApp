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

    public Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id)
    {
        throw new NotImplementedException();
    }
}