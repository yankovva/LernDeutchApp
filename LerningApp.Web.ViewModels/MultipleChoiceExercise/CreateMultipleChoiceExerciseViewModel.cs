using System.ComponentModel.DataAnnotations;

using static LerningApp.Common.EntityValidationConstants.MultipleChoiceExercise;

namespace LerningApp.Web.ViewModels.MultipleChoiceExercise;

public class CreateMultipleChoiceExerciseViewModel
{
    [Required]
    public string LessonId { get; set; } = null!;
    
    [Required]
    [MaxLength(QuestionMaxLength)]
    [MinLength(QuestionMinLength)]
    public string Question { get; set; } = null!;
   
    public IList<MultipleChoiceOptionsAddViewModel> Options { get; set; } = new List<MultipleChoiceOptionsAddViewModel>();
    
    [Range(1,5)]
    public int DifficultyLevel { get; set; }
}