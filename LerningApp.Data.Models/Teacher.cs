namespace LerningApp.Data.Models;

public class Teacher
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public bool IsApproved { get; set; }
    
    public DateTime? TeacherSince { get; set; }
    public string? Qualification { get; set; }
    
    public string? Biography { get; set; }

    public Guid UserId { get; set; } 
    
    public ApplicationUser User { get; set; } = null!;
    
    public virtual ICollection<Course> CreatedCourses { get; set; } = new HashSet<Course>();
    public virtual ICollection<Lesson> CreatedLessons { get; set; } = new HashSet<Lesson>();
    public virtual ICollection<MultipleChoiceExercise> CreatedMultipleChoiceExercises { get; set; } = new HashSet<MultipleChoiceExercise>();
    public virtual ICollection<ListeningExercise> ListeningExercises { get; set; } = new HashSet<ListeningExercise>();
    public virtual ICollection<ListeningQuestion> ListeningQuestions { get; set; } = new HashSet<ListeningQuestion>();
    public virtual ICollection<TranslationExercise> CreatedTranslationExercises { get; set; } = new HashSet<TranslationExercise>();
}