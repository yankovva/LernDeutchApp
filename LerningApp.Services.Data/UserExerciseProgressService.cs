using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data;

public class UserExerciseProgressService(IRepository<UserExerciseProgress, Guid> userExerciseProgressRepository,
    ITeacherService teacherService) : IUserExerciseProgressService
{
    public async Task<ServiceResult> CompleteExerciseAsync(Guid userId, Guid exerciseId)
    {
        var userProgress = await userExerciseProgressRepository
            .GetAllAttached()
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
        List<UserExerciseProgress> exerciseProgresses = new ();
        foreach (var ex in exercises)
        {
            var idStr = exerciseIdSelector(ex);
            if (!Guid.TryParse(idStr, out var exerciseGuid))
                continue;
            
            var hasProgress = await HasUserProgresAsync(userId, idStr);
            if (!hasProgress.Result)
            {
                return false;
            }
            if (!hasProgress.Data)
            {
                exerciseProgresses.Add(new UserExerciseProgress
                {
                    LessonId = lessonId,
                    UserId = Guid.Parse(userId),
                    ExerciseId = Guid.Parse(idStr),
                    ExerciseType = exerciseType
                });
            }
        }
        userExerciseProgressRepository.AddRange(exerciseProgresses);
        await userExerciseProgressRepository.SaveChangesAsync();
        return true;
    }
}