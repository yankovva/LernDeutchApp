namespace LerningApp.Data.Models;

public class UserCourse
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public DateTime StartedAt { get; set; } 
    
    public DateTime? CompletedAt { get; set; }
}