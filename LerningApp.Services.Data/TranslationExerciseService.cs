using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.TranslationExercise;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.Enums;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Exercise;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data;

public class TranslationExerciseService(
    IRepository<TranslationExercise, Guid> exerciseRepository,
    IRepository<Lesson, Guid> lessonRepository,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository,
    ITeacherService teacherService,
    IUserExerciseProgressService userExerciseProgressService,
    UserManager<ApplicationUser> userManager) : ITranslationExerciseService
{
    public async Task<ServiceResultT<CreateTranslationExerciseViewModel>> GetAddTranslationExercisesAsync(string lessonId, string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(InvalidLessonIdMessage,ServiceErrorType.NotFound);
        }
        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonGuid);

        if (lesson == null)
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(LessonNotFoundMessage,ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }

        var model = new CreateTranslationExerciseViewModel()
        {
            LessonId = lessonId
        };
        return ServiceResultT<CreateTranslationExerciseViewModel>.Success(model);
    }

    public async Task<ServiceResult> AddTranslationExerciseAsync(CreateTranslationExerciseViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage,ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage,ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }
        
        TranslationExercise exercise = new TranslationExercise()
        {
            LessonId = Guid.Parse(model.LessonId),
            GermanSentence = model.GermanCorrectTranslation,
            EnglishSentence = model.SentenceEn,
            BulgarianSentence = model.SentenceBg,
            PublisherId = teacherId.Value,
            DifficultyLevel = ExerciseDifficultyLevel.Hard,
        };

        exerciseRepository.Add(exercise);
        await exerciseRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<(bool isCorrect, string correctAnswer)?> CheckTranslationAsync(string exerciseId, string userAnswer, string lessonId, string userId)
    {
        if (!Guid.TryParse(exerciseId, out var exerciseGuidId))
        {
            return null;
        }
        
        if (!Guid.TryParse(lessonId, out var lessonGuidId))
        {
            return null;
        }
        
        var exercise = await exerciseRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(x => x.Id == exerciseGuidId && x.LessonId == lessonGuidId);

        if (exercise == null)
        {
            return null;
        }
        
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (!Guid.TryParse(userId, out Guid userGuidId))
        {
            return null;
        }
        
        var isUnlocked = await userLessonProgressRepository
            .GetAllAttached()
            .AnyAsync(up => up.LessonId == lessonGuidId && up.UserId == Guid.Parse(userId) && up.IsUnlocked == true);

        if (!isUnlocked && !isTeacher)
        {
            return null;
        }

        bool isCorrect = string.Equals(userAnswer?.Trim(), exercise.GermanSentence.Trim(),
            StringComparison.OrdinalIgnoreCase);
        if (isCorrect && !isTeacher)
        {
            var result = await userExerciseProgressService.CompleteExerciseAsync(userGuidId, exercise.Id);
            if (result.Result == false)
            {
                return null;
            }
        }

        return (isCorrect, exercise.GermanSentence);
    }

    public async Task<ServiceResult> SoftDeleteAsync(string exerciseId, string userId)
    {
        if (!Guid.TryParse(exerciseId, out var exerciseGuidId))
        {
            return ServiceResult.Fail(InvalidExerciseIdMessage,ServiceErrorType.NotFound);
        }

        var exercise = await exerciseRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == exerciseGuidId);

        if (exercise == null)
        {
            return ServiceResult.Fail(InvalidExerciseIdMessage,ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }
        
        exercise.IsDeleted = true;
        
        await exerciseRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<EditTranslationExerciseViewModel>> GetEditTranslation(string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid exerciseId))
        {
            return ServiceResultT<EditTranslationExerciseViewModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }

        TranslationExercise? exercise = await exerciseRepository
            .GetAllAttached()
            .Include(e => e.Lesson)
            .FirstOrDefaultAsync(e => e.Id == exerciseId);
        
        if (exercise == null)
        {
            return ServiceResultT<EditTranslationExerciseViewModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResultT<EditTranslationExerciseViewModel>.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }

        var model = new EditTranslationExerciseViewModel()
        {
            Id = exercise.Id.ToString(),
            LessonId = exercise.LessonId.ToString(),
            DifficultyLevel = exercise.DifficultyLevel,
            GermanCorrectTranslation = exercise.GermanSentence,
            SentenceBg = exercise.BulgarianSentence,
            SentenceEn = exercise.EnglishSentence,
        };
        
        return ServiceResultT<EditTranslationExerciseViewModel>.Success(model);
    }

    public async Task<ServiceResult> PostEditranslation(EditTranslationExerciseViewModel model, string userId)
    {
         if (string.IsNullOrWhiteSpace(model.Id) || !Guid.TryParse(model.Id, out Guid exerciseId))
         {
             return ServiceResult.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
         }

         TranslationExercise? exercise = await exerciseRepository
             .GetAllAttached()
             .FirstOrDefaultAsync(e => e.Id == exerciseId);
        
         if (exercise == null)
         {
             return ServiceResult.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
         }
       
         Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
         var user = await userManager.FindByIdAsync(userId);
         bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
         if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
         {
             return ServiceResult.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
         }
        
         exercise.BulgarianSentence = model.SentenceBg;
         exercise.EnglishSentence = model.SentenceEn;
         exercise.GermanSentence = model.GermanCorrectTranslation;
         exercise.DifficultyLevel = model.DifficultyLevel;
        
         await exerciseRepository.SaveChangesAsync();
       
         return ServiceResult.Success();
    }
}