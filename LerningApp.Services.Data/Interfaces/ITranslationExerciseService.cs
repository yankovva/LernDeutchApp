
using LerningApp.Common;
using LerningApp.Web.ViewModels.TranslationExercise;

namespace LerningApp.Services.Data.Interfaces;

public interface ITranslationExerciseService 
{ 
    Task<ServiceResultT<CreateTranslationExerciseViewModel>> GetAddTranslationExercisesAsync(string lessonId, string userId);
    Task<ServiceResult> AddTranslationExerciseAsync(CreateTranslationExerciseViewModel model, string userId);
    Task<(bool isCorrect, string correctAnswer)?> CheckTranslationAsync(string exId, string userAnswer, string lessonId, string userId);
    Task<ServiceResult> SoftDeleteAsync(string exerciseId, string userId);
    Task<ServiceResultT<EditTranslationExerciseViewModel>> GetEditTranslation(string id, string userId);
    Task<ServiceResult> PostEditranslation(EditTranslationExerciseViewModel model, string userId);
}