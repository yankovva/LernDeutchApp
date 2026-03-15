using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.File;
using static LerningApp.Common.EntityErrorMessages.PartOfSpeech;
using static LerningApp.Common.EntityErrorMessages.Card;


using static LerningApp.Common.ApplicationConstants;
namespace LerningApp.Services.Data;

public class VocabularyCardService(IRepository<VocabularyCard,Guid> vocabularyCardRepository,
    IRepository<Lesson,Guid> lessonRepository,
    IRepository<PartOfSpeech, Guid> partOfSpeechrRepository,
    IFileService fileService): IVocabularyCardService
{
    public async Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuidId))
        {
            return ServiceResultT<VocabularyCardsIndexViewModel>.Fail(InvalidLessonIdMessage);
        }
        
        var lesson = await lessonRepository
            .GetByIdAsync(lessonGuidId);
          
        if (lesson == null)
        {
            return ServiceResultT<VocabularyCardsIndexViewModel>.Fail(LessonNotFoundMessage);
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
            return ServiceResultT<VocabularyCardDetailsViewModel>.Fail(InvalidCardIdMessage); 
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
            return ServiceResultT<VocabularyCardDetailsViewModel>.Fail(CardNotFoundMessage);
        }
        
        var de = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "de");
        var en = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "en");
        var bg = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "bg");

        VocabularyCardDetailsViewModel model = new VocabularyCardDetailsViewModel
        {
            Id = card.Id.ToString(),
            LessonId = card.LessonId.ToString(),
            LessonName = card.Lesson?.Name ?? "Урок",
            ImageUrl = card.ImagePath!,
            GermanWord = de!.Word ,
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

    public async Task<ServiceResult> CreateVocabularyCardAsync(VocabularyCardCreateInputModel model)
    {
        if (string.IsNullOrEmpty(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
         return ServiceResult.Fail(InvalidLessonIdMessage, string.Empty);
        }

        if (await lessonRepository.GetByIdAsync(lessonId) == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage, string.Empty);
        }

        if (string.IsNullOrEmpty(model.PartOfSpeechId) || !Guid.TryParse(model.PartOfSpeechId, out Guid partOfSpeechId))
        {
            return ServiceResult.Fail(InvalidPartOfSpeechIdMessage, nameof(model.PartOfSpeechId));
        }

        if (await partOfSpeechrRepository.GetByIdAsync(partOfSpeechId) == null)
        {
            return ServiceResult.Fail(PartOfSpeechNotFoundMessage,nameof(model.PartOfSpeechId));
        }

        string imagePath = string.Empty;

        if (model.Image?.Length > 0)
        {
            string[] allowedExtensions = AllowedImageExtensions;
            long maxSize = MaxFileSize;

            if (!fileService.IsFileValid(model.Image, allowedExtensions, maxSize))
            {
                return ServiceResult.Fail(InvalidFileMessage, nameof(model.Image));

            }
            string extension = Path.GetExtension(model.Image.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            imagePath = await fileService.UploadFileAsync(model.Image, DefaultCardDirectoryPath, uniqueFileName);
        }

        List<VocabularyTerm> terms =
        [
            new VocabularyTerm()
            {
                Word = model.GermanWord,
                Gender = model.Gender,
                ExampleSentence = model.ExampleSentence,
                Side = "de",
                IsPrimary = true,
            },

            new VocabularyTerm()
            {
                Word = model.BulgarianWord,
                Side = "bg",
                IsPrimary = true,
            },

            new VocabularyTerm()
            {
                Word = model.EnglishWord,
                Side = "en",
                IsPrimary = true,
            }
        ];

        var newCard = new VocabularyCard()
        {
            LessonId = lessonId,
            PartOfSpeechId = partOfSpeechId,
            Terms = terms,
            ImagePath = string.IsNullOrEmpty(imagePath) ? null : imagePath
        };
         
        vocabularyCardRepository.Add(newCard);
        await vocabularyCardRepository.SaveChangesAsync();
        
        return ServiceResult.Success();        
    }

    public async Task<ServiceResultT<VocabularyCardEditInputModel>> GetCardEditByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(InvalidCardIdMessage);
        }
        
        var card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .Include(c => c.PartOfSpeech)
            .FirstOrDefaultAsync(c => c.Id == cardId);
       
        if (card == null)
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(CardNotFoundMessage);
        }
        
        var de = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "de");
        var en = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "en");
        var bg = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "bg");

        var model = new VocabularyCardEditInputModel()
        {
            Id = id,
            LessonId = card.LessonId.ToString(),
            PartOfSpeechId = card.PartOfSpeechId.ToString(),
            GermanWord = de!.Word ,
            EnglishWord = en!.Word,
            BulgarianWord = bg!.Word,
            ExampleSentence = de!.ExampleSentence,
            Gender = de!.Gender,
        };
        
        return ServiceResultT<VocabularyCardEditInputModel>.Success(model);
    }

    public async Task<ServiceResult> PostCardEditByIdAsync(VocabularyCardEditInputModel model, string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResult.Fail(InvalidCardIdMessage);
        }
        
        var card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .Include(c => c.PartOfSpeech)
            .FirstOrDefaultAsync(c => c.Id == cardId);
       
        if (card == null)
        {
            return ServiceResult.Fail(CardNotFoundMessage);
        }
        
        if (string.IsNullOrEmpty(model.PartOfSpeechId) || !Guid.TryParse(model.PartOfSpeechId, out Guid partOfSpeechId))
        {
            return ServiceResult.Fail(InvalidPartOfSpeechIdMessage,nameof(model.PartOfSpeechId));
        }

        var partOfSpeech = await partOfSpeechrRepository
            .GetByIdAsync(partOfSpeechId);
       
        if (partOfSpeech == null)
        {
            return ServiceResult.Fail(PartOfSpeechNotFoundMessage, nameof(model.PartOfSpeechId));
        }
        
        if (model.Image?.Length > 0)
        {
            string[] allowedExtensions = AllowedImageExtensions;
            long maxSize = MaxFileSize;

            if (!fileService.IsFileValid(model.Image, allowedExtensions, maxSize))
            {
                return ServiceResult.Fail(InvalidFileMessage, nameof(model.Image));
            }

            string extension = Path.GetExtension(model.Image.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string imagePath = await fileService.UploadFileAsync(model.Image, DefaultCardDirectoryPath, uniqueFileName);
           
            if (card.ImagePath != null)
            {
                fileService.DeleteFile(card.ImagePath);
            }

            card.ImagePath = string.IsNullOrEmpty(imagePath) ? null : imagePath;
        }

        card.PartOfSpeechId = partOfSpeechId;
        
        var de = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "de");
        var en = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "en");
        var bg = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "bg");
        
        de!.Word = model.GermanWord;
        en!.Word = model.EnglishWord;
        bg!.Word = model.BulgarianWord;
        de!.ExampleSentence = model.ExampleSentence;
        de!.Gender = model.Gender;
        
        await vocabularyCardRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteCardByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResult.Fail(InvalidCardIdMessage);
        }

        var card = await vocabularyCardRepository
            .GetByIdAsync(cardId);
        if (card == null)
        {
            return ServiceResult.Fail(CardNotFoundMessage);
        }

        if (card.ImagePath != null)
        {
            fileService.DeleteFile(card.ImagePath);
        }
        vocabularyCardRepository.DeleteByEntity(card);
        await vocabularyCardRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }
    public async Task<ServiceResult> SoftDeleteCardAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResult.Fail(InvalidCardIdMessage);
        }

        VocabularyCard? card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return ServiceResult.Fail(CardNotFoundMessage);
        }
        
        card.IsDeleted = true;

        foreach (var term in card.Terms)
        {
            term.IsDeleted = true;
        }
        
        await vocabularyCardRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}