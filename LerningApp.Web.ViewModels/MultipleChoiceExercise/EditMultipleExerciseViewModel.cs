using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.ViewModels.MultipleChoiceExercise;

using static LerningApp.Common.Enums;
using static LerningApp.Common.EntityValidationConstants.MultipleChoiceExercise;
public class EditMultipleExerciseViewModel
{
    [Required]
    public string Id { get; set; } = null!;
    [Required]
    public string LessonId { get; set; } = null!;
    [Required]
    [MaxLength(QuestionMaxLength)]
    [MinLength(QuestionMinLength)]
    public string Question { get; set; } = null!;
    
    public IList<EditMultipleChoiceOptionsViewModel> Options { get; set; } = new List<EditMultipleChoiceOptionsViewModel>();
   
    public ExerciseDifficultyLevel DifficultyLevel { get; set; }
}