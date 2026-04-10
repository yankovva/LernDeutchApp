using LerningApp.Common;
using LerningApp.Contracts.MultipleChoiceExerciseDtos;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Exercise;
using static LerningApp.Common.ApplicationConstants;


namespace LerningApp.Services.Data;

public class MultipleChoiceExerciseService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<MultipleChoiceExercise, Guid> exerciseRepository,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository,
    ITeacherService teacherService,
    IUserExerciseProgressService userExerciseProgressService,
    UserManager<ApplicationUser> userManager) : IMultipleChoiceExerciseService
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
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Fail(AccessDeniedMessage);
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
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        var filledOptions = model.Options
            .Where(o => !string.IsNullOrWhiteSpace(o.AnswerText))
            .ToList();
        
        if (filledOptions.Count <= 1)
        {
            return ServiceResult.Fail("Add at least 2 options");
        }

        if (filledOptions.Count(o => o.IsCorrect) != 1)
        {
            return ServiceResult.Fail("Add 1 correct option");
        }
        
        var options = new List<MultipleChoiceExerciseOption>();
        foreach (var option in model.Options)
        {
            if (!string.IsNullOrWhiteSpace(option.AnswerText))
            {
                options.Add( new MultipleChoiceExerciseOption()
                {
                    Answer = option.AnswerText,
                    IsCorrect = option.IsCorrect,
                    OrderIndex = option.OrderIndex,
                });
            }
        }
        
        MultipleChoiceExercise exercise = new MultipleChoiceExercise()
        {
            LessonId = lessonId,
            DifficultyLevel = model.DifficultyLevel,
            Options = options,
            Question = model.Question,
            PublisherId = teacherId.Value,
        };
        
        exerciseRepository.Add(exercise);
        await exerciseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<MultipleChoiceCheckResultDto>> CheckMultipleChoice(CheckMultipleChoiceExerciseInputDto dto, string userId)
    {
        if (!Guid.TryParse(dto.ExerciseId, out var exerciseGuidId))
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(ExerciseNotFoundMessage);
        }
        
        if (!Guid.TryParse(dto.LessonId, out var lessonGuidId))
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(LessonNotFoundMessage);
        }
        
        var exercise = await exerciseRepository
            .GetAllAttached()
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == exerciseGuidId && x.LessonId == lessonGuidId);

        if (exercise == null)
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(ExerciseNotFoundMessage);
        }
        
        if (!Guid.TryParse(userId, out Guid userGuidId))
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(AccessDeniedMessage);
        }
        
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        
        var isUnlocked = await userLessonProgressRepository
            .GetAllAttached()
            .AnyAsync(up => up.LessonId == lessonGuidId && up.UserId == Guid.Parse(userId) && up.IsUnlocked == true);
      
        if (!isUnlocked && !isTeacher)
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(AccessDeniedMessage);
        }
        
        var correctOption = exercise.Options
            .FirstOrDefault(x => x.IsCorrect)!;

        var resultDto = new MultipleChoiceCheckResultDto()
        {
            CorrectAnswer = correctOption.Answer,
        };
        
       if (correctOption.Answer == dto.SelectedAnswer)
        {
            if (!isTeacher)
            {
                var result = await userExerciseProgressService.CompleteExerciseAsync(userGuidId, exercise.Id);
                if (result.Result == false)
                {
                    return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(InvalidOperationMessage);
                }
            }
            
            resultDto.IsCorrect = true;
        }
       
        return ServiceResultT<MultipleChoiceCheckResultDto>.Success(resultDto);
    }

    public async Task<ServiceResult> SoftDeleteExerciseAsync(string id, string userId)
    {
        if (!Guid.TryParse(id, out var exerciseGuidId))
        {
            return ServiceResult.Fail(InvalidExerciseIdMessage);
        }

        var exercise = await exerciseRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == exerciseGuidId);

        if (exercise == null)
        {
            return ServiceResult.Fail(InvalidExerciseIdMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        exercise.IsDeleted = true;

        await exerciseRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
}