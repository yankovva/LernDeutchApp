using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.Enums;

namespace LerningApp.Data.Models;

public class TranslationExercise
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Comment("The german sentence for translation")]
    public string GermanSentence { get; set; } = null!;
   
    [Comment("The chosen translation language option bg / en")]
    public TranslationLanguage ChosenTranslationLanguage { get; set; }
    
    [Comment("The correct translated sentence in the chosen language")]
    public string CorrectTranslation { get; set; }  = null!;
    
    [Comment("The order of the exercise")]
    public int OrderIndex { get; set; }
    
    [Comment("Shows if the exercise is deleted")]
    public bool IsDeleted { get; set; } 
   
    [Comment("Foreign key to Lesson")]
    public Guid LessonId { get; set; }
   
    [Comment("Lesson Reference")]
    public Lesson Lesson { get; set; } = null!;
}