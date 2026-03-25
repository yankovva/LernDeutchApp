namespace LerningApp.Contracts.MultipleChoiceExerciseDtos;

public class CheckMultipleChoiceExerciseInputDto
{
    public string ExerciseId { get; set; } = null!;
    public string SelectedAnswer { get; set; } = null!;
    public string LessonId { get; set; }= null!;
}