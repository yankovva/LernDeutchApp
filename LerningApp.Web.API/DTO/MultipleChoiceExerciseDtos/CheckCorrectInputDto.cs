using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.API.DTO.MultipleChoiceExerciseDtos;

public class CheckCorrectInputDto
{
    [Required]
    public string ExerciseId { get; set; } = null!;
    [Required]
    public string LessonId { get; set; }= null!;
    [Required]
    public string SelectedAnswer { get; set; }= null!;
}