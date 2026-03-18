namespace LerningApp.Web.ViewModels.ListeningExercise;

public class ListeningAnswerInputModel
{
    public Guid QuestionId { get; set; }
    public string SelectedAnswer { get; set; } = null!;
}