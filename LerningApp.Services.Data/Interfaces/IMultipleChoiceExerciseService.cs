using System.Text.Json.Nodes;
using LerningApp.Common;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;

namespace LerningApp.Services.Data.Interfaces;

public interface IMultipleChoiceExerciseService
{
    public Task<ServiceResultT<CreateMultipleChoiceExerciseViewModel>> GetCreateAsync(string lessonId, string userId);
    public Task<ServiceResult> CreateAsync(CreateMultipleChoiceExerciseViewModel model, string userId);
    public Task<(bool isCorrect, string correctAnswer)?> CheckMultipleChoice(string exerciseId, string selectedAnswer);
}