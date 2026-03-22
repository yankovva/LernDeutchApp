using static LerningApp.Common.Enums;

namespace LerningApp.Data.Models;

public class UserExerciseProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid ExerciseId { get; set; }
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public ExerciseType ExerciseType { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}