using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.TranslationExercise;

namespace LerningApp.Services.Data;

public class TranslationExerciseService(
    IRepository<TranslationExercise, Guid> exerciseRepository,
    IRepository<Lesson, Guid> lessonRepository,
    ITeacherService teacherService) : ITranslationExerciseService
{
    public async Task<ServiceResultT<CreateTranslationExerciseViewModel>> GetAddTranslationExercisesAsync(string lessonId, string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail("Невалиден урок.");
        }
        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonGuid);

        if (lesson == null)
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail("Невалиден урок.");
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail("Нямате права.");
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
            return ServiceResult.Fail("Invalid Lesson");
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return ServiceResult.Fail("Invalid Lesson");
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResultT<CreateTranslationExerciseViewModel>.Fail("Нямате права.");
        }
        
        TranslationExercise exercise = new TranslationExercise()
        {
            LessonId = Guid.Parse(model.LessonId),
            GermanSentence = model.GermanCorrectTranslation,
            EnglishSentence = model.SentenceEn,
            BulgarianSentence = model.SentenceBg,
            OrderIndex = model.OrderIndex,
            PublisherId = teacherId.Value,
            DifficultyLevel = model.DifficultyLevel,
        };

        await exerciseRepository.AddAsync(exercise);
        return ServiceResult.Success();
    }

    public async Task<(bool isCorrect, string correctAnswer)?> CheckTranslationAsync(string exerciseId, string userAnswer)
    {
        if (string.IsNullOrWhiteSpace(exerciseId) || !Guid.TryParse(exerciseId, out Guid exId))
        {
            return null;
        }
        var exercise = await exerciseRepository
            .GetByIdAsync(exId);
        
        if (exercise == null)
        {
            return null;
        }

        bool isCorrect = string.Equals(userAnswer?.Trim(), exercise.GermanSentence.Trim(),
            StringComparison.OrdinalIgnoreCase);

        return (isCorrect, exercise.GermanSentence);
    }

}