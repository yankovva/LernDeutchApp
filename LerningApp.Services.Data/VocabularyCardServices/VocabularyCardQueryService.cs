using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;

using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Card;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data.VocabularyCardServices;

public class VocabularyCardQueryService(
    IRepository<VocabularyCard, Guid> vocabularyCardRepository,
    IRepository<Lesson, Guid> lessonRepository,
    IUserLessonProgressService userLessonProgressService) : IVocabularyCardQueryService
{
    public async Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId, string userId)
    {
        var result = await userLessonProgressService
            .IsLessonUnlockedForAUserAsync(lessonId, userId);

        if (!result.Result || !result.Data)
        {
            return ServiceResultT<VocabularyCardsIndexViewModel>.Fail(result.Message ?? "Invalid operation.", ServiceErrorType.AccessDenied);
        }

        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuidId))
        {
            return ServiceResultT<VocabularyCardsIndexViewModel>.Fail(InvalidLessonIdMessage, ServiceErrorType.NotFound);
        }

        var lesson = await lessonRepository
            .GetByIdAsync(lessonGuidId);

        if (lesson == null)
        {
            return ServiceResultT<VocabularyCardsIndexViewModel>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
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

    public async Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid cardGuid))
        {
            return ServiceResultT<VocabularyCardDetailsViewModel>.Fail(InvalidCardIdMessage, ServiceErrorType.NotFound);
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
            return ServiceResultT<VocabularyCardDetailsViewModel>.Fail(CardNotFoundMessage, ServiceErrorType.NotFound);
        }

        var result = await userLessonProgressService
            .IsLessonUnlockedForAUserAsync(card.LessonId.ToString(), userId);
        if (result.Data == false)
        {
            return ServiceResultT<VocabularyCardDetailsViewModel>.Fail(result.Message ?? "Invalid operation.", ServiceErrorType.AccessDenied);
        }

        var de = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "de");
        var en = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "en");
        var bg = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "bg");

        VocabularyCardDetailsViewModel model = new VocabularyCardDetailsViewModel
        {
            Id = card.Id.ToString(),
            LessonId = card.LessonId.ToString(),
            LessonName = card.Lesson?.Name ?? "Урок",
            ImageUrl = card.ImagePath,
            GermanWord = de!.Word,
            BulgarianTranslation = bg?.Word ?? "no word",
            EnglishTranslation = en?.Word ?? "no word",
            PartOfSpeech = card.PartOfSpeech.Name,
            BulgarianSynonyms = card.Terms.Where(t => t.Side == "bg" && !t.IsPrimary)
                .Select(t => t.Word)
                .ToList(),
            EnglishSynonyms = card.Terms
                .Where(t => t.Side == "en" && !t.IsPrimary)
                .Select(t => t.Word)
                .ToList(),
            Gender = de.Gender ?? "-",
            ExampleSentence = de.ExampleSentence ?? "-",
        };

        return ServiceResultT<VocabularyCardDetailsViewModel>.Success(model);
    }
}
