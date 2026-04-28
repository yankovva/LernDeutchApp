using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.ViewModels.ListeningExercise;

public class EditListeningQuestionInputModel
{
    [StringLength(250, MinimumLength = 5)]
    public string? QuestionText { get; set; } 
    public List<EditListeningQuestionOptionInputModel> Options { get; set; } = new();

}