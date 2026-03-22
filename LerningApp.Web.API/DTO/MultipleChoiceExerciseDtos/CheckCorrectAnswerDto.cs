namespace LerningApp.Web.API.DTO.MultipleChoiceExerciseDtos;

public class CheckCorrectAnswerDto
{
    public bool IsCorrect {get;set;}

    public string CorrectAnswer { get; set; } = null!;
}