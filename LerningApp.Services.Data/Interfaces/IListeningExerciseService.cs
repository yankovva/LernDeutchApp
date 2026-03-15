using LerningApp.Common;
using LerningApp.Web.ViewModels.ListeningExercise;

namespace LerningApp.Services.Data.Interfaces;

public interface IListeningExerciseService
{
     Task<ServiceResultT<CreateListeningExerciseViewModel>> CreateGetListeningExercise(string lessonId, string userId);
     
     Task<ServiceResult> CreatePostListeningExercise(CreateListeningExerciseViewModel model, string userId);
     
     public Task<(bool isCorrect, string correctAnswer)?> CheckListeningExerciseAnswer(string exerciseId, string selectedAnswer,string lessonId, string userId);
     
}