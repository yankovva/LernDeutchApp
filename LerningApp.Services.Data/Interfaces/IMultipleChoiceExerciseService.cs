using System.Text.Json.Nodes;
using LerningApp.Common;
using LerningApp.Contracts.MultipleChoiceExerciseDtos;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;

namespace LerningApp.Services.Data.Interfaces;

public interface IMultipleChoiceExerciseService
{
     Task<ServiceResultT<CreateMultipleChoiceExerciseViewModel>> GetCreateAsync(string lessonId, string userId);
     Task<ServiceResult> CreateAsync(CreateMultipleChoiceExerciseViewModel model, string userId);
     Task<ServiceResultT<MultipleChoiceCheckResultDto>> CheckMultipleChoice(CheckMultipleChoiceExerciseInputDto dto, string userId);
     Task<ServiceResult> SoftDeleteExerciseAsync(string id, string userId);
}