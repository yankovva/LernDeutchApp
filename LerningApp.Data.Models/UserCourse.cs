using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class UserCourse
{
    [Comment("Foreign key to user")]
    public Guid UserId { get; set; }

    [Comment("Navigation to user")]
    public ApplicationUser User { get; set; } = null!;
    
    [Comment("Foreign key to Course")]
    public Guid CourseId { get; set; }
    
    [Comment("Navigation to Course")]
    public Course Course { get; set; } = null!;
    
    [Comment("Course started at (UTC)")]
    public DateTime StartedAt { get; set; } 
    
    [Comment("Course completed at (UTC)")]
    public DateTime? CompletedAt { get; set; }
}