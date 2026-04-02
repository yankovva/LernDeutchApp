namespace LerningApp.Web.ViewModels.Teacher;

public class CourseTeacherIndexViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string LevelName { get; set; } = null!;
    public decimal Price { get; set; }
    public int EnrolledStudentsCount { get; set; }
    public bool IsPublished { get; set; }


}