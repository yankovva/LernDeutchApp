using LerningApp.Common;
using LerningApp.Contracts.ListeningExerciseDtos;
using LerningApp.Web.ViewModels.ListeningExercise;

namespace LerningApp.Services.Data.Interfaces;

public interface IListeningExerciseService
{
     Task<ServiceResultT<CreateListeningExerciseViewModel>> CreateGetListeningExercise(string lessonId, string userId);
     Task<ServiceResult> CreatePostListeningExercise(CreateListeningExerciseViewModel model, string userId);
     Task<ServiceResultT<List<ListeningQuestionCheckResultDto>>> CheckListeningExerciseAnswer(CheckListeningExerciseInputDto dto, string userId);
     Task<ServiceResult> SoftDeleteExerciseAsync(string exerciseId, string userId);
     Task<ServiceResultT<EditListeningExerciseViewModel>> GetEditListeningExercise(string id, string userId);
     Task<ServiceResult> PostEditListeningExercise(EditListeningExerciseViewModel model, string userId);
     Task<ServiceResultT<EditListeningQuestionInputModel>> GetEditListeningQuestion(string id, string userId);
     Task<ServiceResult> PostEditListeningQuestion(EditListeningQuestionInputModel model, string userId);
     Task<ServiceResult> DeleteOptionAsync (DeleteListeningOptionViewModel model, string userId);
     Task<ServiceResult> SoftDeleteQuestionAsync (string id, string userId);
}