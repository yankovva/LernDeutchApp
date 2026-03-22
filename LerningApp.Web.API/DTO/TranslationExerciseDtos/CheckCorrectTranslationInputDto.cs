using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.API.DTO.TranslationExerciseDtos;

public class CheckCorrectTranslationInputDto
{
    [Required]
    public string ExerciseId { get; set; } = null!;
    [Required]
    public string LessonId { get; set; }= null!;
    [Required]
    public string UserTranslation { get; set; }= null!;
}