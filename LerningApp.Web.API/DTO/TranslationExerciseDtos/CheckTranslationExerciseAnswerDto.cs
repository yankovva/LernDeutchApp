namespace LerningApp.Web.API.DTO.TranslationExerciseDtos;

public class CheckTranslationExerciseAnswerDto
{
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = null!;
}