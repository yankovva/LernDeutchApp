namespace LerningApp.Web.ViewModels.Course;

public class CourseDetailsViewModel
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string LevelName { get; set; } = null!;
    
    public bool IsActive { get; set; }
    
}