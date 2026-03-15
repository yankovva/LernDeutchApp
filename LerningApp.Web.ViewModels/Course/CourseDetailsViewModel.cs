namespace LerningApp.Web.ViewModels.Course;

public class CourseDetailsViewModel
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string LevelName { get; set; } = null!;
    
    public double ProgressPercentage { get; set; }
    public bool IsActive { get; set; }
    
    public decimal Price { get; set; }
    
    public bool IsEnrolled { get; set; }
    
    public string PublisherId { get; set; } = null!;
    
    public int TotalWordsInCourse { get; set; }

    public virtual IList<CourseLessonsViewModel> CourseLessons { get; set; } =
        new List<CourseLessonsViewModel>();

}