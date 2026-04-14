namespace LerningApp.Web.ViewModels.Admin.Teacher;

public class AdminTeacherIndexViewModel
{
    public string UserId { get; set; } = null!;
    
    public string TeacherId { get; set; } = null!;
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Email { get; set; }
    
    public bool HasPendingProfileChanges { get; set; }
    public string Status { get; set; } = null!;
    
    public string? TeacherSince { get; set; }
}