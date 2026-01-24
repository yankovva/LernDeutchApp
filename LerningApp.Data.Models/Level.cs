namespace LerningApp.Data.Models;

public class Level
{
    public Level()
    {
        this.Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;

    public ICollection<Course> CoursesForLevel { get; set; } = new List<Course>();
}
