namespace LerningApp.Data.Models;

public class UserLessonProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsCompleted { get; set; }
    public bool IsUnlocked  { get; set; }
    public DateTime? CompletedAt { get; set; } 
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}