using System.ComponentModel.DataAnnotations;
using LerningApp.Web.ViewModels.Course;
using static LerningApp.Common.EntityValidationConstants.Lesson;
namespace LerningApp.Web.ViewModels.Lesson;

public class AddLessonInputModel
{
    [Required]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MinLength(ContentMinLength)]
    [MaxLength(ContentMaxLength)]
    public string Content { get; set; } = null!;
    
    [Required]
    [MinLength(TargetMinLength)]
    [MaxLength(TargetMaxLength)]
    public string Target { get; set; } = null!;

    [Range(OrderIndexMin, OrderIndexMax)]
    public int OrderIndex { get; set; }

    // optional
    public string? CourseId { get; set; }

    public IList<CourseOptionsViewModel> Courses { get; set; }
        = new List<CourseOptionsViewModel>();
}