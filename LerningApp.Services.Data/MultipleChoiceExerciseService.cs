using System.Text.Json.Nodes;
using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;

namespace LerningApp.Services.Data;

public class MultipleChoiceExerciseService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<MultipleChoiceExercise, Guid> exerciseRepository) : IMultipleChoiceExerciseService
{
    public async Task<ServiceResult> CreateAsync(CreateMultipleChoiceExerciseViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
                return ServiceResult.Fail("Невалиден урок.");
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);
        
            if (lesson == null)
            {
                return ServiceResult.Fail("Невалиден урок.");

            }
        
            MultipleChoiceExercise exercise = new MultipleChoiceExercise()
        {
            LessonId = lessonId,
            Question = model.Question,
            CorrectAnswer = model.CorrectAnswer,
            SecondWrongAnswer = model.SecondWrongAnswer ?? null,
            FirstWrongAnswer = model.FirstWrongAnswer,
            ThirdWrongAnswer = model.ThirdWrongAnswer ?? null,
            OrderIndex = model.OrderIndex,
            PublisherId = Guid.Parse(userId),
        };
        
            await exerciseRepository.AddAsync(exercise);
            return ServiceResult.Success();
    }

    public async Task<(bool isCorrect, string correctAnswer)?> CheckMultipleChoice(string exerciseId, string selectedAnswer)
    {
        if (!Guid.TryParse(exerciseId, out var exId))
        {
            return null;
        }
        
        var exercise = await exerciseRepository
            .GetByIdAsync(exId);

        if (exercise == null)
        {
            return null;
        }
        
        bool isCorrect;
        if (exercise.CorrectAnswer == selectedAnswer)
        {
            isCorrect = true;
        } else
            isCorrect = false;
        
        return (isCorrect, exercise.CorrectAnswer);
    }
}