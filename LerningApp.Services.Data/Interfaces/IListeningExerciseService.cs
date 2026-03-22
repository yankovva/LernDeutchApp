using LerningApp.Common;
using LerningApp.Contracts.ListeningExerciseDtos;
using LerningApp.Web.ViewModels.ListeningExercise;

namespace LerningApp.Services.Data.Interfaces;

public interface IListeningExerciseService
{
     Task<ServiceResultT<CreateListeningExerciseViewModel>> CreateGetListeningExercise(string lessonId, string userId);
     
     Task<ServiceResult> CreatePostListeningExercise(CreateListeningExerciseViewModel model, string userId);

     Task<ServiceResultT<List<ListeningQuestionCheckResultDto>>> CheckListeningExerciseAnswer(
         CheckListeningExerciseInputDto dto, string userId);
}