namespace LerningApp.Web.ViewModels.ListeningExercise.DTOs;

public class ListeningQuestionCheckResultDTO
{
    public string QuestionId { get; set; } = null!;
    
    public bool IsCorrect { get; set; }
}