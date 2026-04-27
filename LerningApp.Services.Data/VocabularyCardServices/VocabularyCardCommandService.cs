using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.ApplicationConstants;
using static LerningApp.Common.EntityErrorMessages.Card;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.File;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.PartOfSpeech;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data.VocabularyCardServices;

public class VocabularyCardCommandService(
    IRepository<VocabularyCard, Guid> vocabularyCardRepository,
    IRepository<Lesson, Guid> lessonRepository,
    IRepository<PartOfSpeech, Guid> partOfSpeechRepository,
    ITeacherService teacherService,
    IFileService fileService,
    IPartOfSpeechService partOfSpeechService,
    UserManager<ApplicationUser> userManager) : IVocabularyCardCommandService
{
    public async Task<ServiceResultT<VocabularyCardCreateInputModel>> GetCreateVocabularyCardAsync(string lessonId, string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<VocabularyCardCreateInputModel>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository.GetByIdAsync(lessonGuid);
        if (lesson == null)
        {
            return ServiceResultT<VocabularyCardCreateInputModel>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<VocabularyCardCreateInputModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
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
        if (string.IsNullOrEmpty(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage, ServiceErrorType.NotFound);
        }

        var lesson = await lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        if (string.IsNullOrEmpty(model.PartOfSpeechId) || !Guid.TryParse(model.PartOfSpeechId, out Guid partOfSpeechId))
        {
            return ServiceResult.Fail(InvalidPartOfSpeechIdMessage,ServiceErrorType.Validation, nameof(model.PartOfSpeechId));
        }

        if (await partOfSpeechRepository.GetByIdAsync(partOfSpeechId) == null)
        {
            return ServiceResult.Fail(PartOfSpeechNotFoundMessage, ServiceErrorType.Validation, nameof(model.PartOfSpeechId));
        }

        string imagePath = string.Empty;

        if (model.Image?.Length > 0)
        {
            if (!fileService.IsFileValid(model.Image, AllowedImageExtensions, MaxFileSize))
            {
                return ServiceResult.Fail(InvalidFileMessage, ServiceErrorType.Validation,nameof(model.Image));
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
            PublisherId = teacherId!.Value,
            Terms = terms,
            ImagePath = string.IsNullOrEmpty(imagePath) ? null : imagePath
        };

        vocabularyCardRepository.Add(newCard);
        await vocabularyCardRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<VocabularyCardEditInputModel>> GetCardEditByIdAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(InvalidCardIdMessage,ServiceErrorType.NotFound);
        }

        var card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .Include(c => c.Lesson)
            .Include(c => c.PartOfSpeech)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(CardNotFoundMessage, ServiceErrorType.NotFound);
        }
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || card.PublisherId != teacherId))
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
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
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResult.Fail(InvalidCardIdMessage, ServiceErrorType.NotFound);
        }

        var card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .Include(c => c.Lesson)
            .Include(c => c.PartOfSpeech)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return ServiceResult.Fail(CardNotFoundMessage, ServiceErrorType.NotFound);
        }

        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || card.PublisherId != teacherId))
        {
            return ServiceResultT<VocabularyCardEditInputModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        if (string.IsNullOrEmpty(model.PartOfSpeechId) || !Guid.TryParse(model.PartOfSpeechId, out Guid partOfSpeechId))
        {
            return ServiceResult.Fail(InvalidPartOfSpeechIdMessage, ServiceErrorType.Validation,nameof(model.PartOfSpeechId));
        }

        var partOfSpeech = await partOfSpeechRepository.GetByIdAsync(partOfSpeechId);

        if (partOfSpeech == null)
        {
            return ServiceResult.Fail(PartOfSpeechNotFoundMessage, ServiceErrorType.Validation,nameof(model.PartOfSpeechId));
        }

        if (model.Image?.Length > 0)
        {
            if (!fileService.IsFileValid(model.Image, AllowedImageExtensions, MaxFileSize))
            {
                return ServiceResult.Fail(InvalidFileMessage, ServiceErrorType.Validation, nameof(model.Image));
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
