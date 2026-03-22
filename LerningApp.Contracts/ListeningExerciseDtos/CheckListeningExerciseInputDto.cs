namespace LerningApp.Contracts.ListeningExerciseDtos;

public class CheckListeningExerciseInputDto
{
    public string ExerciseId { get; set; } = null!;
    public  string LessonId { get; set; } = null!;
    public List<ListeningAnswerInputDto> Answers { get; set; } = new List<ListeningAnswerInputDto>();
}