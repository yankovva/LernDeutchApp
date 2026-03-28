namespace LerningApp.Web.ViewModels.Admin.Teacher;

public class AdminTeacherIndexViewModel
{
    public string Id { get; set; } = null!;
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Email { get; set; }
    
    public bool IsApproved { get; set; }
    
    public string TeacherSince { get; set; }
}