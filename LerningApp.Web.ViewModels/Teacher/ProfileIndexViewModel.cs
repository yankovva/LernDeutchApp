namespace LerningApp.Web.ViewModels.Teacher;

public class ProfileIndexViewModel
{
    public string TeacherId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ProfileImage { get; set; }

    public string Status { get; set; } = null!;

    public string? TeacherSince { get; set; }

    public string? Biography { get; set; }

    public string? Qualifications { get; set; }
}