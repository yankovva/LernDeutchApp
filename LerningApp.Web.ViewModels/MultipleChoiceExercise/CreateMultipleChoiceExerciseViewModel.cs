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