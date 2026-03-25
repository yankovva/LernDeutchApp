namespace LerningApp.Contracts.MultipleChoiceExerciseDtos;

public class MultipleChoiceCheckResultDto
{
    public string CorrectAnswer { get; set; } = null!;
    
    public bool IsCorrect { get; set; }
}