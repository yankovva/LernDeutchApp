using System.ComponentModel.DataAnnotations;

using static LerningApp.Common.EntityValidationConstants.TranslationExercise;

namespace LerningApp.Web.ViewModels.TranslationExercise;

public class CreateTranslationExerciseViewModel
{
    [Required]
    public string LessonId { get; set; } = null!;
    
    [Required]
    [MinLength(SentenceMinLength)]
    [MaxLength(SentenceMaxLength)]
    public string GermanCorrectTranslation { get; set; } = null!;
    
    [Required]
    [MinLength(SentenceMinLength)]
    [MaxLength(SentenceMaxLength)]
    public string SentenceEn { get; set; } = null!;
    
    [Required]
    [MinLength(SentenceMinLength)]
    [MaxLength(SentenceMaxLength)]
    public string SentenceBg { get; set; } = null!;
    
    [Range(1,5)]
    public int DifficultyLevel { get; set; }
    public int OrderIndex { get; set; }
}