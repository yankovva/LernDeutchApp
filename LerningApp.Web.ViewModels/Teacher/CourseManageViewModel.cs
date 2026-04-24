using LerningApp.Common;

namespace LerningApp.Web.ViewModels.Teacher;

public class CourseManageViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Goal { get; set; }
    public string? LevelName { get; set; }
    public decimal Price { get; set; }
    public string CreatedAt { get; set; } = null!;
    public Enums.CourseStatus Status { get; set; }

    public int LessonsCount { get; set; }
    public int TotalWordsCount { get; set; }
    public int TotalExercisesCount { get; set; }
    public int EnrolledStudentsCount { get; set; }

    public IEnumerable<CourseManageLessonViewModel> Lessons { get; set; }
        = new List<CourseManageLessonViewModel>();
}