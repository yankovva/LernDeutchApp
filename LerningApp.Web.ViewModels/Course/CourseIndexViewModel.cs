namespace LerningApp.Web.ViewModels.Course;

public class CourseIndexViewModel
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
   public int LessonsCount { get; set; }
   
   public string CourseLevel { get; set; } = null!;
}