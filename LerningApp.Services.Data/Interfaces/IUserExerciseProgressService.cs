using LerningApp.Common;
using LerningApp.Data.Models;

namespace LerningApp.Services.Data.Interfaces;

public interface IUserExerciseProgressService
{
    Task<ServiceResult> CompleteExerciseAsync(Guid userId, Guid exerciseId);
    Task<ServiceResultT<bool>> HasUserProgresAsync(string userId, string exerciseId);

    Task<bool> CreateUserExerciseProgress<T>(IEnumerable<T> exercises, Func<T, string> exerciseIdSelector, string userId, Guid lessonId, Enums.ExerciseType exerciseType);
}