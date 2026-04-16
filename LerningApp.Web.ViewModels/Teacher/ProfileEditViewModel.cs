using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using static LerningApp.Common.EntityValidationConstants.Teacher;

namespace LerningApp.Web.ViewModels.Teacher;

public class ProfileEditViewModel
{
    [Required]
    public string TeacherId { get; set; } = null!;
    [Required]
    public string UserId { get; set; } = null!;
    [Required]
    [MinLength(FirstNameMinLength)]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;
    [Required]
    [MinLength(LastNameMinLength)]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;
    [MinLength(PhoneNumberMinLength)]
    [MaxLength(PhoneNumberMaxLength)]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? ProfileImage { get; set; }
    
    public IFormFile? Image { get; set; } 

    [Required]
    [MinLength(BiographyMinLength)]
    [MaxLength(BiographyMaxLength)]
    public string Biography { get; set; }= null!;
    
    [Required]
    [MinLength(QualificationMinLength)]
    [MaxLength(QualificationMaxLength)]
    public string Qualifications { get; set; } = null!;
}