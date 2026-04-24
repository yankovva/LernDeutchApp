namespace LerningApp.Web.ViewModels.Teacher;

public class CourseManageLessonViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Target { get; set; }
    public int OrderIndex { get; set; }
}