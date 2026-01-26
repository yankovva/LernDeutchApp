using System.ComponentModel.DataAnnotations;
using LerningApp.Web.ViewModels.Level;
using static LerningApp.Common.EntityValidationConstants.Course;
namespace LerningApp.Web.ViewModels.Course;

public class AddCourseViewModel
{
    [Required]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    
    [Required]
    [MinLength(DescriptionMinLength)]
    [MaxLength(DescriptionMaxLength)]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "Моля избери ниво.")]
    public Guid? LevelId { get; set; }  

    public IEnumerable<LevelOptionsViewModel> Levels { get; set; } = new List<LevelOptionsViewModel>();

}