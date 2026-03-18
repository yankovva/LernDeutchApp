using LerningApp.Common;

namespace LerningApp.Services.Data.Interfaces;

public interface IUserExerciseProgressService
{
    Task<ServiceResult> CompleteExerciseAsync(Guid userId, Guid exerciseId);
}