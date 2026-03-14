using LerningApp.Web.ViewModels.UserLessonProgress;

namespace LerningApp.Web.ViewModels.Course;

public class CourseLessonsViewModel
{
    public string LessinId { get; set; } = null!;

    public string LessonName { get; set; } = null!;
    
    public string LessonTarget { get; set; } = null!;
    
    public int WordsInLesson { get; set; }
    
    public IndexUserLessonProgressViewModel UserLessonProgress { get; set; } = new IndexUserLessonProgressViewModel();
}