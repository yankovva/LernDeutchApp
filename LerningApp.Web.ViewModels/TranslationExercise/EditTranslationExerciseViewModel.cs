using System.ComponentModel.DataAnnotations;
using LerningApp.Common;

namespace LerningApp.Web.ViewModels.TranslationExercise;

using static LerningApp.Common.Enums;
using static LerningApp.Common.EntityValidationConstants.TranslationExercise;
public class EditTranslationExerciseViewModel
{
    [Required]
    public string Id { get; set; } = null!;
    
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
    public ExerciseDifficultyLevel DifficultyLevel { get; set; }
}