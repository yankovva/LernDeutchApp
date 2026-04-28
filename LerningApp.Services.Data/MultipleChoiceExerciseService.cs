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
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data;

public class MultipleChoiceExerciseService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<MultipleChoiceExercise, Guid> exerciseRepository,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository,
    IRepository<MultipleChoiceExerciseOption, Guid> optionRepository,
    ITeacherService teacherService,
    IUserExerciseProgressService userExerciseProgressService,
    UserManager<ApplicationUser> userManager) : IMultipleChoiceExerciseService
{
    public async  Task<ServiceResultT<CreateMultipleChoiceExerciseViewModel>> GetCreateAsync(string lessonId, string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Fail("Невалиден урок.", ServiceErrorType.NotFound);
        }
        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonGuid);

        if (lesson == null)
        {
            return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Fail("Невалиден урок.", ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<CreateMultipleChoiceExerciseViewModel>.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
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
                return ServiceResult.Fail(InvalidLessonIdMessage, ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);
        
        if (lesson == null)
        {
                return ServiceResult.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }
        
        var filledOptions = model.Options
            .Where(o => !string.IsNullOrWhiteSpace(o.AnswerText))
            .ToList();
        
        if (filledOptions.Count <= 1)
        {
            return ServiceResult.Fail("Add at least 2 options", ServiceErrorType.General);
        }

        if (filledOptions.Count(o => o.IsCorrect) != 1)
        {
            return ServiceResult.Fail("Add 1 correct option", ServiceErrorType.General);
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
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        if (!Guid.TryParse(dto.LessonId, out var lessonGuidId))
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        var exercise = await exerciseRepository
            .GetAllAttached()
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == exerciseGuidId && x.LessonId == lessonGuidId);

        if (exercise == null)
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        if (!Guid.TryParse(userId, out Guid userGuidId))
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        
        var isUnlocked = await userLessonProgressRepository
            .GetAllAttached()
            .AnyAsync(up => up.LessonId == lessonGuidId && up.UserId == Guid.Parse(userId) && up.IsUnlocked == true);
      
        if (!isUnlocked && !isTeacher)
        {
            return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
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
                var result = await userExerciseProgressService
                    .CompleteExerciseAsync(userGuidId, exercise.Id);
                if (result.Result == false)
                {
                    return ServiceResultT<MultipleChoiceCheckResultDto>.Fail(InvalidOperationMessage,ServiceErrorType.General);
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
            return ServiceResult.Fail(InvalidExerciseIdMessage, ServiceErrorType.NotFound);
        }

        var exercise = await exerciseRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == exerciseGuidId);

        if (exercise == null)
        {
            return ServiceResult.Fail(InvalidExerciseIdMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        exercise.IsDeleted = true;

        await exerciseRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
    
    public async Task<ServiceResultT<EditMultipleExerciseViewModel>> GetEditMultipleChoice (string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid exerciseId))
        {
            return ServiceResultT<EditMultipleExerciseViewModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }

        MultipleChoiceExercise? exercise = await exerciseRepository
            .GetAllAttached()
            .Include(e => e.Lesson)
            .Include(e => e.Options)
            .FirstOrDefaultAsync(e => e.Id == exerciseId);
        
        if (exercise == null)
        {
            return ServiceResultT<EditMultipleExerciseViewModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResultT<EditMultipleExerciseViewModel>.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }

        var model = new EditMultipleExerciseViewModel()
        {
            Id = exercise.Id.ToString(),
            LessonId = exercise.LessonId.ToString(),
            Question = exercise.Question,
            DifficultyLevel = exercise.DifficultyLevel,
            Options = exercise.Options
                .OrderBy(op => op.OrderIndex)
                .Select(op => new EditMultipleChoiceOptionsViewModel()
                {
                    IsCorrect = op.IsCorrect,
                    AnswerText = op.Answer,
                    OrderIndex = op.OrderIndex,
                }).ToList()
        };
        
        return ServiceResultT<EditMultipleExerciseViewModel>.Success(model);
    }

    public async Task<ServiceResult> PostEditMultipleChoice(EditMultipleExerciseViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.Id) || !Guid.TryParse(model.Id, out Guid exerciseId))
        {
            return ServiceResult.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }

        MultipleChoiceExercise? exercise = await exerciseRepository
            .GetAllAttached()
            .Include(e => e.Options)
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
        
        exercise.Question = model.Question;
        exercise.DifficultyLevel = model.DifficultyLevel;
        
        var modelOptins = model.Options
            .Where(o => !string.IsNullOrWhiteSpace(o.AnswerText))
            .ToList();
        
        if (modelOptins.Count <= 1)
        {
            return ServiceResult.Fail("Add at least 2 options", ServiceErrorType.General);
        }

        if (modelOptins.Count(o => o.IsCorrect) != 1)
        {
            return ServiceResult.Fail("Add 1 correct option", ServiceErrorType.General);
        }
        
        foreach (var oldOption in exercise.Options.ToList())
        {
             optionRepository.DeleteByEntity(oldOption);
        }
        
        await optionRepository.SaveChangesAsync();

        var newOptions = new List<MultipleChoiceExerciseOption>();
        
        foreach (var option in model.Options)
        {
            if (!string.IsNullOrWhiteSpace(option.AnswerText))
            {
                var newOPtion = new MultipleChoiceExerciseOption
                {
                    Id = Guid.NewGuid(),
                    MultipleChoiceExerciseId = exerciseId,
                    IsCorrect = option.IsCorrect,
                    Answer = option.AnswerText,
                    OrderIndex = option.OrderIndex
                };
                newOptions.Add(newOPtion);
            }
        }
        optionRepository.AddRange(newOptions);
        exercise.Options = newOptions;
        await exerciseRepository.SaveChangesAsync();
       
        return ServiceResult.Success();
    }
}