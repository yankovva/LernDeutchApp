using System.ComponentModel.DataAnnotations;
using LerningApp.Web.ViewModels.Level;
using static LerningApp.Common.EntityValidationConstants.Course;
using static LerningApp.Common.EntityValidationMessages.Course;

namespace LerningApp.Web.ViewModels.Course;

public class CourseEditViewModel
{
    public string Id { get; set; } = null!;

    [Required (ErrorMessage = CourseNameValidationMessage)]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = CourseDescriptionValidationMessage)]
    [MinLength(DescriptionMinLength)]
    [MaxLength(DescriptionMaxLength)]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = CourseLevelValidationMessage)]
    public string LevelId { get; set; } = null!;

    public IEnumerable<LevelOptionsViewModel> Levels { get; set; } 
        = new HashSet<LevelOptionsViewModel>();
}