using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

using static LerningApp.Common.EntityValidationConstants.ListeningExercise;

namespace LerningApp.Web.ViewModels.ListeningExercise;

public class CreateListeningExerciseViewModel
{ 
     [Required]
     public string LessonId { get; set; } = null!;
     
     [Required]
     [Range(1,5)]
     public int DifficultyLevel { get; set; }
     
     [Required]
     public IFormFile AudioFile { get; set; } = null!;

     public List<CreateListeningQuestionInputModel> Questions { get; set; } = new ();
}