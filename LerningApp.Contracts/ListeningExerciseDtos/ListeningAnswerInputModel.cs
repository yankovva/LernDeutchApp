namespace LerningApp.Contracts.ListeningExerciseDtos;

public class ListeningAnswerInputDto
{
    public Guid QuestionId { get; set; }
    public string SelectedAnswer { get; set; } = null!;
}