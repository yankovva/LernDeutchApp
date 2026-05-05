using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.ViewModels.ListeningExercise;
using static LerningApp.Common.EntityValidationConstants.ListeningExercise;
public class AddListeningQuestionOptionInputModel
{
    [Required]
    [MinLength(AnswerMinLength)]
    [MaxLength(AnswerMaxLength)]
    public string? AnswerText { get; set; }
    
    [Required]
    public int OrderIndex { get; set; }
}