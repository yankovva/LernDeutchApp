using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

using static LerningApp.Common.EntityValidationConstants.ListeningExercise;

namespace LerningApp.Web.ViewModels.ListeningExercise;

public class CreateListeningExerciseViewModel
{
    public string LessonId { get; set; } = null!;
    
    public string Question { get; set; } = null!;
    
    public int DifficultyLevel { get; set; }
    
    public IFormFile AudioFile { get; set; } = null!;
    
    [Required]
    [MaxLength(AnswerMaxLength)]
    [MinLength(AnswerMinLength)]
    public string CorrectAnswer { get; set; } = null!;
    
    [Required]
    [MaxLength(AnswerMaxLength)]
    [MinLength(AnswerMinLength)]
    public string FirstWrongAnswer { get; set; } = null!;
    
    [MaxLength(AnswerMaxLength)]
    [MinLength(AnswerMinLength)]
    public string? SecondWrongAnswer { get; set; }
    
    [MaxLength(AnswerMaxLength)]
    [MinLength(AnswerMinLength)]
    public string? ThirdWrongAnswer { get; set; }
    public int OrderIndex { get; set; }
}