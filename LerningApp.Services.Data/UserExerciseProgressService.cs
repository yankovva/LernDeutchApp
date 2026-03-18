using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data;

public class UserExerciseProgressService(IRepository<UserExerciseProgress, Guid> userExerciseProgressRepository) : IUserExerciseProgressService
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
}