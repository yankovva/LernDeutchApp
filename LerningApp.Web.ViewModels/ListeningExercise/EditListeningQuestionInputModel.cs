using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.ViewModels.ListeningExercise;

public class EditListeningQuestionInputModel
{
    [Required]
    public string Id { get; set; } = null!;
    [Required]
    public string ExerciseId { get; set; } = null!;
    [StringLength(250, MinimumLength = 5)]
    public string? QuestionText { get; set; } 
    public int CorrectOptionIndex { get; set; }
    public List<EditListeningQuestionOptionInputModel> Options { get; set; } = new();
}
