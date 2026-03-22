namespace LerningApp.Contracts.ListeningExerciseDtos;

public class ListeningQuestionCheckResultDto
{
    public string QuestionId { get; set; } = null!;
    
    public bool IsCorrect { get; set; }
}