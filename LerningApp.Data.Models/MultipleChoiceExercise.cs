using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class MultipleChoiceExercise
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Comment("The difficulty level of the exercise")]
    public int DifficultyLevel { get; set; }
    public bool IsDeleted { get; set; } 
   
    [Comment("Foreign key to Lesson")]
    public Guid LessonId { get; set; }
   
    [Comment("Lesson Reference")]
    public Lesson Lesson { get; set; } = null!;
    
    public string Question { get; set; } = null!;
    
    [Comment("Foreign key to ApplicationUser")]
    public Guid PublisherId { get; set; }
   
    [Comment("ApplicationUser Reference")]
    public Teacher Publisher { get; set; } = null!;
    
    public ICollection<MultipleChoiceExerciseOption> Options { get; set; } 
        = new HashSet<MultipleChoiceExerciseOption>();
}