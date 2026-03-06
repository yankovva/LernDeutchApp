using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class MultipleChoiceExercise
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Comment("The Question for the exercise")]
    public string Question { get; set; } = null!;
    
    [Comment("The Correct answer of the exercise")]
    public string CorrectAnswer { get; set; }= null!;
    
    [Comment("The First wrong answer of the exercise")]
    public string FirstWrongAnswer { get; set; }= null!;
   
    [Comment("The Second wrong answer of the exercise")]
    public string? SecondWrongAnswer { get; set; }
   
    [Comment("The Third wrong answer of the exercise if needed")]
    public string? ThirdWrongAnswer { get; set; }
    
    [Comment("The difficulty level of the exercise")]
    public int DifficultyLevel { get; set; }
    public bool IsDeleted { get; set; } 
   
    [Comment("Foreign key to Lesson")]
    public Guid LessonId { get; set; }
   
    [Comment("Lesson Reference")]
    public Lesson Lesson { get; set; } = null!;
    
    [Comment("Foreign key to ApplicationUser")]
    public Guid PublisherId { get; set; }
   
    [Comment("ApplicationUser Reference")]
    public Teacher Publisher { get; set; } = null!;
}