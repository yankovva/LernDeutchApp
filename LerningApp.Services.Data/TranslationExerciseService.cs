using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.TranslationExercise;

namespace LerningApp.Services.Data;

public class TranslationExerciseService(
    IRepository<TranslationExercise, Guid> exerciseRepository,
    IRepository<Lesson, Guid> lessonRepository) : ITranslationExerciseService
{
    public async Task<ServiceResult> AddTranslationExerciseAsync(CreateTranslationExerciseViewModel model, Guid userId)
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

        TranslationExercise exercise = new TranslationExercise()
        {
            LessonId = Guid.Parse(model.LessonId),
            GermanSentence = model.GermanCorrectTranslation,
            EnglishSentence = model.SentenceEn,
            BulgarianSentence = model.SentenceBg,
            OrderIndex = model.OrderIndex,
            PublisherId = userId,
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