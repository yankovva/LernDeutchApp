using System.Text.Json.Nodes;
using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;

namespace LerningApp.Services.Data;

public class MultipleChoiceExerciseService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<MultipleChoiceExercise, Guid> exerciseRepository,
    ITeacherService teacherService) : IMultipleChoiceExerciseService
{
    public async  Task<ServiceResultT<CreateMultipleChoiceExerciseViewModel>> GetCreateAsync(string lessonId, string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Fail("Невалиден урок.");
        }
        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonGuid);

        if (lesson == null)
        {
            return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Fail("Невалиден урок.");
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Fail("Нямате права.");
        }
       
        var model = new CreateMultipleChoiceExerciseViewModel()
        {
            LessonId = lessonId
        };
        
        return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Success(model);
    }

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
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResult.Fail("Нямате права.");
        }
        
        MultipleChoiceExercise exercise = new MultipleChoiceExercise()
        {
            LessonId = lessonId,
            Question = model.Question,
            CorrectAnswer = model.CorrectAnswer,
            SecondWrongAnswer = model.SecondWrongAnswer ?? null,
            FirstWrongAnswer = model.FirstWrongAnswer,
            ThirdWrongAnswer = model.ThirdWrongAnswer ?? null,
            DifficultyLevel = model.DifficultyLevel,
            OrderIndex = model.OrderIndex,
            PublisherId = teacherId.Value,
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