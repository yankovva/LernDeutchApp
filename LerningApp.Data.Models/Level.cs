namespace LerningApp.Data.Models;

public class Level
{
   
    public Guid Id { get; set; }= Guid.NewGuid();

    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;

    public virtual ICollection<Course> CoursesForLevel { get; set; } = new HashSet<Course>();
}
