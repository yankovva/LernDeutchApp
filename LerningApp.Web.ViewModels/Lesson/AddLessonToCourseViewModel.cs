using System.ComponentModel.DataAnnotations;
using LerningApp.Web.ViewModels.Course;

using static LerningApp.Common.EntityValidationConstants.Lesson;
namespace LerningApp.Web.ViewModels.Lesson;

public class AddLessonToCourseViewModel
{
    [Required]
    public string LessonId { get; set; } = null!;
    
    [Required]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string LessonName { get; set; } = null!;
    
    public string? SelectedCourseId { get; set; } 
    
    public List<CourseCheckBoxItemInputModel> Courses { get; set; }
        = new List<CourseCheckBoxItemInputModel>();
}