using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class ListeningExercise
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Comment("The Question for the exercise")]
    public string Question { get; set; } = null!;
    
    [Comment("The Audio path of the exercise")]

    public string AudioPath { get; set; } = null!;
    
    [Comment("The difficulty level of the exercise")]
    public int DifficultyLevel { get; set; }
    
    [Comment("Shows if the exercise is deleted")]
    public bool IsDeleted { get; set; } 
    
    [Comment("Foreign key to Lesson")]
    public Guid LessonId { get; set; }
   
    [Comment("Lesson Reference")]
    public Lesson Lesson { get; set; } = null!;
    
    [Comment("Foreign key to ApplicationUser")]
    public Guid PublisherId { get; set; }
   
    [Comment("ApplicationUser Reference")]
    public Teacher Publisher { get; set; } = null!;

    public ICollection<ListeningExerciseOption> AnswerOptions { get; set; } 
        = new HashSet<ListeningExerciseOption>();
}