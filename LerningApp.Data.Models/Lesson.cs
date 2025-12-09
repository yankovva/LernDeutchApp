namespace LerningApp.Data.Models;

public class Lesson{

    public Lesson()
    {
        this.Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string ContentHtml { get; set; } = null!;
    
    public int OrderIndex { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string CourseId { get; set; } 
    
    public Course Course { get; set; }
}