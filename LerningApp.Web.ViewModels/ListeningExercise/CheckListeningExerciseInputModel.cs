namespace LerningApp.Web.ViewModels.ListeningExercise;

public class CheckListeningExerciseInputModel
{
    public string ExerciseId { get; set; } = null!;
    public  string LessonId { get; set; } = null!;
    
    public List<ListeningAnswerInputModel> Answers { get; set; } = new List<ListeningAnswerInputModel>();
}