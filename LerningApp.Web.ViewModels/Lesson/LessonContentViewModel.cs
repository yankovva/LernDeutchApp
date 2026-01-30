namespace LerningApp.Web.ViewModels.Lesson;

public class LessonContentViewModel
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public string Content { get; set; } = null!;
    
    public int OrderIndex { get; set; }
    
    public string? CourseId { get; set; }
    
    public string? CourseName { get; set; }

    public int WordCount { get; set; }
    
}