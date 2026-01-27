namespace LerningApp.Web.ViewModels.Lesson;

public class LessonIndexViewModel
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string CreatedAt{ get; set; } = null!;
    public string? CourseName { get; set; }
    
    public string? CourseId { get; set; }
    
    public string? LevelName { get; set; }
}