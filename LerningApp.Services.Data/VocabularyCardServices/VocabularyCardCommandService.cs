using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;

using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.ApplicationConstants;
using static LerningApp.Common.EntityErrorMessages.Card;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.File;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.PartOfSpeech;

namespace LerningApp.Services.Data.VocabularyCardServices;

public class VocabularyCardCommandService(
    IRepository<VocabularyCard, Guid> vocabularyCardRepository,
    IRepository<Lesson, Guid> lessonRepository,
    IRepository<PartOfSpeech, Guid> partOfSpeechRepository,
    ITeacherService teacherService,
    IFileService fileService,
    IPartOfSpeechService partOfSpeechService) : IVocabularyCardCommandService
{
    public async Task<ServiceResultT<VocabularyCardCreateInputModel>> GetCreateVocabularyCardAsync(string lessonId, string userId)
    {
        var teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null)
        {
            return ServiceResultT<VocabularyCardCreateInputModel>.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<VocabularyCardCreateInputModel>.Fail(LessonNotFoundMessage);
        }

        Lesson? lesson = await lessonRepository.GetByIdAsync(lessonGuid);
        if (lesson == null)
        {
            return ServiceResultT<VocabularyCardCreateInputModel>.Fail(LessonNotFoundMessage);
        }

        if (lesson.PublisherId != teacherId)
        {
            return ServiceResultT<VocabularyCardCreateInputModel>.Fail(AccessDeniedMessage);
        }

        var model = new VocabularyCardCreateInputModel()
        {
            LessonId = lessonId,
            PartOfSpeechOptions = await partOfSpeechService.GetAllPartOfSpeechOptionsAsync()
        };
        return ServiceResultT<VocabularyCardCreateInputModel>.Success(model);
    }

    public async Task<ServiceResult> CreateVocabularyCardAsync(VocabularyCardCreateInputModel model, string userId)
    {
        var teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrEmpty(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage, string.Empty);
        }

        var lesson = await lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage, string.Empty);
        }

        if (lesson.PublisherId != teacherId)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrEmpty(model.PartOfSpeechId) || !Guid.TryParse(model.PartOfSpeechId, out Guid partOfSpeechId))
        {
            return ServiceResult.Fail(InvalidPartOfSpeechIdMessage, nameof(model.PartOfSpeechId));
        }

        if (await partOfSpeechRepository.GetByIdAsync(partOfSpeechId) == null)
        {
            return ServiceResult.Fail(PartOfSpeechNotFoundMessage, nameof(model.PartOfSpeechId));
        }

        string imagePath = string.Empty;

        if (model.Image?.Length > 0)
        {
            if (!fileService.IsFileValid(model.Image, AllowedImageExtensions, MaxFileSize))
            {
                return ServiceResult.Fail(InvalidFileMessage, nameof(model.Image));
            }

            string extension = Path.GetExtension(model.Image.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            imagePath = await fileService.UploadFileAsync(model.Image, DefaultCardDirectoryPath, uniqueFileName);
        }

        List<VocabularyTerm> terms =
        [
            new VocabularyTerm
            {
                Word = model.GermanWord,
                Gender = model.Gender,
                ExampleSentence = model.ExampleSentence,
                Side = "de",
                IsPrimary = true,
            },
            new VocabularyTerm
            {
                Word = model.BulgarianWord,
                Side = "bg",
                IsPrimary = true,
            },
            new VocabularyTerm
            {
                Word = model.EnglishWord,
                Side = "en",
                IsPrimary = true,
            }
        ];

        var newCard = new VocabularyCard
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

    public async Task<ServiceResultT<VocabularyCardEditInputModel>> GetCardEditByIdAsync(string id, string userId)
    {
        var teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null)
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(InvalidCardIdMessage);
        }

        var card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .Include(c => c.Lesson)
            .Include(c => c.PartOfSpeech)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(CardNotFoundMessage);
        }

        if (card.Lesson.PublisherId != teacherId)
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(AccessDeniedMessage);
        }

        var de = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "de");
        var en = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "en");
        var bg = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "bg");

        var model = new VocabularyCardEditInputModel()
        {
            Id = id,
            LessonId = card.LessonId.ToString(),
            PartOfSpeechId = card.PartOfSpeechId.ToString(),
            GermanWord = de!.Word,
            EnglishWord = en!.Word,
            BulgarianWord = bg!.Word,
            ExampleSentence = de.ExampleSentence,
            Gender = de.Gender,
        };

        return ServiceResultT<VocabularyCardEditInputModel>.Success(model);
    }

    public async Task<ServiceResult> PostCardEditByIdAsync(VocabularyCardEditInputModel model, string id, string userId)
    {
        var teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResult.Fail(InvalidCardIdMessage);
        }

        var card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .Include(c => c.Lesson)
            .Include(c => c.PartOfSpeech)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return ServiceResult.Fail(CardNotFoundMessage);
        }

        if (card.Lesson.PublisherId != teacherId)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrEmpty(model.PartOfSpeechId) || !Guid.TryParse(model.PartOfSpeechId, out Guid partOfSpeechId))
        {
            return ServiceResult.Fail(InvalidPartOfSpeechIdMessage, nameof(model.PartOfSpeechId));
        }

        var partOfSpeech = await partOfSpeechRepository.GetByIdAsync(partOfSpeechId);

        if (partOfSpeech == null)
        {
            return ServiceResult.Fail(PartOfSpeechNotFoundMessage, nameof(model.PartOfSpeechId));
        }

        if (model.Image?.Length > 0)
        {
            if (!fileService.IsFileValid(model.Image, AllowedImageExtensions, MaxFileSize))
            {
                return ServiceResult.Fail(InvalidFileMessage, nameof(model.Image));
            }

            string extension = Path.GetExtension(model.Image.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string imagePath = await fileService.UploadFileAsync(model.Image, DefaultCardDirectoryPath, uniqueFileName);
            string? oldImagePath = card.ImagePath;

            card.ImagePath = string.IsNullOrEmpty(imagePath) ? null : imagePath;
            await vocabularyCardRepository.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(oldImagePath))
            {
                fileService.DeleteFile(oldImagePath);
            }
        }

        card.PartOfSpeechId = partOfSpeechId;

        var de = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "de");
        var en = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "en");
        var bg = card.Terms.FirstOrDefault(t => t.IsPrimary && t.Side == "bg");

        de!.Word = model.GermanWord;
        en!.Word = model.EnglishWord;
        bg!.Word = model.BulgarianWord;
        de.ExampleSentence = model.ExampleSentence;
        de.Gender = model.Gender;

        await vocabularyCardRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
