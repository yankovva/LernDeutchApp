using LerningApp.Common;
using LerningApp.Web.ViewModels.ListeningExercise;
using LerningApp.Web.ViewModels.ListeningExercise.DTOs;

namespace LerningApp.Services.Data.Interfaces;

public interface IListeningExerciseService
{
     Task<ServiceResultT<CreateListeningExerciseViewModel>> CreateGetListeningExercise(string lessonId, string userId);
     
     Task<ServiceResult> CreatePostListeningExercise(CreateListeningExerciseViewModel model, string userId);

     Task<ServiceResultT<List<ListeningQuestionCheckResultDTO>>> CheckListeningExerciseAnswer(
          CheckListeningExerciseInputModel model, string userId);
}