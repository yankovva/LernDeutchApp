using System.ComponentModel.DataAnnotations;

using static LerningApp.Common.EntityValidationConstants.Course;
namespace LerningApp.Web.ViewModels.Course;

public class CourseCheckBoxItemInputModel
{
    [Required]
    public string CourseId { get; set; } = null!;
    [Required]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string CourseName { get; set; } = null!;
}