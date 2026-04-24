namespace LerningApp.Web.ViewModels.Teacher;

public class LessonTeacherIndexViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public int ExercisesCount { get; set; }
}