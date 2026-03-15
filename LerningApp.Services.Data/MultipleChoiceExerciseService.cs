using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.Common;

namespace LerningApp.Services.Data;

public class MultipleChoiceExerciseService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<MultipleChoiceExercise, Guid> exerciseRepository,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository,
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
            return ServiceResult.Fail(AccessDeniedMessage);
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
            PublisherId = teacherId.Value,
        };
        
        exerciseRepository.Add(exercise);
        await exerciseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<(bool isCorrect, string correctAnswer)?> CheckMultipleChoice(string exerciseId, string selectedAnswer,string lessonId,string userId)
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
        
        bool isCorrect;
        if (exercise.CorrectAnswer == selectedAnswer)
        {
            isCorrect = true;
        } else
            isCorrect = false;
        
        return (isCorrect, exercise.CorrectAnswer);
    }
}