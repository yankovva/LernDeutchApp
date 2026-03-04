
using LerningApp.Common;
using LerningApp.Web.ViewModels.TranslationExercise;

namespace LerningApp.Services.Data.Interfaces;

public interface ITranslationExerciseService 
{ 
    Task<ServiceResult> AddTranslationExerciseAsync(CreateTranslationExerciseViewModel model, string userId);

    Task<(bool isCorrect, string correctAnswer)?> CheckTranslationAsync(string exId, string userAnswer);
}