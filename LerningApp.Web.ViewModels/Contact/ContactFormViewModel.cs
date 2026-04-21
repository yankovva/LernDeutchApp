using System.ComponentModel.DataAnnotations;

namespace LerningApp.Web.ViewModels.Contact;

public class ContactFormViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    [MinLength(3)]
    [MaxLength(80)]
    public string Name { get; set; } = null!;
    
    [Required]
    [MaxLength(120)]
    [MinLength(3)]
    public string Subject { get; set; } = null!;
    
    [Required]
    [MaxLength(2000)]
    [MinLength(10)]
    public string Message { get; set; } = null!;
}