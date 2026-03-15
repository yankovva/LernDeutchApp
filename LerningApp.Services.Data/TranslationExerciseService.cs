using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.TranslationExercise;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.Common;

namespace LerningApp.Services.Data;

public class TranslationExerciseService(
    IRepository<TranslationExercise, Guid> exerciseRepository,
    IRepository<Lesson, Guid> lessonRepository,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository,
    ITeacherService teacherService) : ITranslationExerciseService
{
    public async Task<ServiceResultT<CreateTranslationExerciseViewModel>> GetAddTranslationExercisesAsync(string lessonId, string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(InvalidLessonIdMessage);
        }
        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonGuid);

        if (lesson == null)
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(LessonNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(AccessDeniedMessage);
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
            return ServiceResult.Fail(InvalidLessonIdMessage);
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail(AccessDeniedMessage);
        }
        
        TranslationExercise exercise = new TranslationExercise()
        {
            LessonId = Guid.Parse(model.LessonId),
            GermanSentence = model.GermanCorrectTranslation,
            EnglishSentence = model.SentenceEn,
            BulgarianSentence = model.SentenceBg,
            PublisherId = teacherId.Value,
            DifficultyLevel = model.DifficultyLevel,
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
        
        var isUnlocked = await userLessonProgressRepository
            .GetAllAttached()
            .AnyAsync(up => up.LessonId == lessonGuidId && up.UserId == Guid.Parse(userId) && up.IsUnlocked == true);
      
        if (!isUnlocked && !isTeacher)
        {
            return null;
        }
        
        bool isCorrect = string.Equals(userAnswer?.Trim(), exercise.GermanSentence.Trim(),
            StringComparison.OrdinalIgnoreCase);

        return (isCorrect, exercise.GermanSentence);
    }

}