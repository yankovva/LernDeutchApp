using System.ComponentModel.DataAnnotations;
using LerningApp.Common;
using Microsoft.AspNetCore.Http;

namespace LerningApp.Web.ViewModels.ListeningExercise;

public class EditListeningExerciseViewModel
{
    [Required]
    public string Id { get; set; } = null!;
    
    [Required]
    public string LessonId { get; set; } = null!;
    public Enums.ExerciseDifficultyLevel DifficultyLevel { get; set; }
     
    [Required]
    public string? AudioPath { get; set; }
    
    public IFormFile? AudioFile { get; set; }
    
    public List<EditListeningQuestionInputModel> Questions { get; set; } = new();
}