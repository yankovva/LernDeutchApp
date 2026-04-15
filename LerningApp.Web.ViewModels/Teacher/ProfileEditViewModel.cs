namespace LerningApp.Web.ViewModels.Teacher;

public class ProfileEditViewModel
{
    public string TeacherId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? ProfileImage { get; set; }

    public string Biography { get; set; }= null!;

    public string Qualifications { get; set; } = null!;
}