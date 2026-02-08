using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class Level
{
    [Comment("PK Unique Identifier")]
    public Guid Id { get; set; }= Guid.NewGuid();
    
    [Comment("The Name of the Level")]
    public string Name { get; set; } = null!;
    
    [Comment("The Description of the Level")]
    public string Description { get; set; } = null!;
    
    [Comment("Courses with this Level")]
    public virtual ICollection<Course> CoursesForLevel { get; set; } = new HashSet<Course>();
}
