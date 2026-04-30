using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.ViewModels.ListeningExercise;

public class EditListeningQuestionOptionInputModel
{
    [Required]
    public string Id { get; set; } = null!;
    
    [StringLength(100, MinimumLength = 1)]
    public string? AnswerText { get; set; } 

    public bool IsCorrect { get; set; }

    public int OrderIndex { get; set; }
}