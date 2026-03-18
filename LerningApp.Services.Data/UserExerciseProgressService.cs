using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.UserLessonProgress;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data;

public class UserExerciseProgressService(IRepository<UserExerciseProgress, Guid> userExerciseProgressRepository,
    ITeacherService teacherService,
    IUserLessonProgressService userLessonProgressService) : IUserExerciseProgressService
{
    public async Task<ServiceResult> CompleteExerciseAsync(Guid userId, Guid exerciseId)
    {
        var userProgress = await userExerciseProgressRepository
            .GetAllAttached()
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(u => u.ExerciseId == exerciseId && u.UserId == userId);
       
        if (userProgress == null)
        {
            return ServiceResult.Fail(InvalidOperationMessage);
        }
        
        if (userProgress.IsCompleted)
            return ServiceResult.Success();
       
        userProgress.CompletedAt = DateTime.UtcNow;
        userProgress.IsCompleted = true;
       
        await userExerciseProgressRepository.SaveChangesAsync();
        await userLessonProgressService.TryCompleteLessonProgressAsync(userProgress.LessonId, userId);
         return ServiceResult.Success();
    }
    public async  Task<ServiceResultT<bool>> HasUserProgresAsync(string userId, string exerciseId)
    {
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (isTeacher)
        {
            return ServiceResultT<bool>.Success(true);
        }
        Guid? userGuid = Guid.TryParse(userId, out Guid parsedUserId) ? parsedUserId : null;
        if (userGuid == null)
        {
            return ServiceResultT<bool>.Fail("Invalid operation.");
        }
        
        Guid? exerciseGuid = Guid.TryParse(exerciseId, out Guid parsedExerciseId) ? parsedExerciseId : null;
        
        if (exerciseGuid == null)
        {
            return ServiceResultT<bool>.Fail("Invalid operation.");
        }

        bool hasProgress = await userExerciseProgressRepository
            .GetAllAttached()
            .AnyAsync(u => u.UserId == userGuid && u.ExerciseId == exerciseGuid);
        
        return ServiceResultT<bool>.Success(hasProgress);
    }

    public async Task<bool> CreateUserExerciseProgress<T>(IEnumerable<T> exercises,  Func<T, string> exerciseIdSelector, string userId, Guid lessonId, ExerciseType exerciseType)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return false;
        }
        
        var allExerciseIds = new List<Guid>();
        foreach (var exercise in exercises)
        {
            var idString = exerciseIdSelector(exercise);
            if (Guid.TryParse(idString, out var exerciseId))
            {
                allExerciseIds.Add(exerciseId);
            }
        }

        if (allExerciseIds.Count == 0)
        {
            return true;
        }

        var existingProgressExerciseIds = await userExerciseProgressRepository
            .GetAllAttached()
            .Where(p =>
                p.UserId == userGuid &&
                p.LessonId == lessonId &&
                p.ExerciseType == exerciseType &&
                allExerciseIds.Contains(p.ExerciseId))
            .Select(p => p.ExerciseId)
            .ToHashSetAsync();
        
        var progressesToCreate = new List<UserExerciseProgress>();
        foreach (var exerciseId in allExerciseIds)
        {
            if (!existingProgressExerciseIds.Contains(exerciseId))
            {
                progressesToCreate.Add(new UserExerciseProgress
                {
                    UserId = userGuid,
                    LessonId = lessonId,
                    ExerciseId = exerciseId,
                    ExerciseType = exerciseType
                });
            }
        }

        if (progressesToCreate.Count > 0)
        {
            userExerciseProgressRepository.AddRange(progressesToCreate);
            await userExerciseProgressRepository.SaveChangesAsync();
        }

        return true;
    }
}