using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.Enums;

namespace LerningApp.Data.Models;

public class TranslationExercise
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Comment("The german sentence")]
    public string GermanSentence { get; set; } = null!;
    
    [Comment("The bulgarian sentence")]
    public string BulgarianSentence { get; set; }  = null!;
    
    [Comment("The english sentence")]
    public string EnglishSentence { get; set; }  = null!;
    
    [Comment("The order of the exercise")]
    public int OrderIndex { get; set; }
    
    [Comment("Shows if the exercise is deleted")]
    public bool IsDeleted { get; set; } 
    
    [Comment("The difficulty level of the exercise")]
    public int DifficultyLevel { get; set; }
   
    [Comment("Foreign key to Lesson")]
    public Guid LessonId { get; set; }
   
    [Comment("Lesson Reference")]
    public Lesson Lesson { get; set; } = null!;
    
    [Comment("Foreign key to ApplicationUser")]
    public Guid PublisherId { get; set; }
   
    [Comment("ApplicationUser Reference")]
    public ApplicationUser Publisher { get; set; } = null!;
}