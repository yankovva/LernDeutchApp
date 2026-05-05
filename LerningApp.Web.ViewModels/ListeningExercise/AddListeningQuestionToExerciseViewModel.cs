using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.ViewModels.ListeningExercise;
using static LerningApp.Common.EntityValidationConstants.ListeningExercise;

public class AddListeningQuestionToExerciseViewModel
{
    public string ExerciseId { get; set; } = null!;
    
    [Required]
    [MinLength(QuestionMinLength)]
    [MaxLength(QuestionMaxLength)]
    public string QuestionText { get; set; } = null!;
    
    [Required]
    public int CorrectOptionIndex { get; set; }
    
    public List<AddListeningQuestionOptionInputModel> Options { get; set; } = new();
}