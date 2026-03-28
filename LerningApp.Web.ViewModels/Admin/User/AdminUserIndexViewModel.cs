namespace LerningApp.Web.ViewModels.Admin.User;

public class AdminUserIndexViewModel
{
    public string Id { get; set; } = null!;
    public string? Email { get; set; } 
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string RegisteredOn { get; set; } = null!;
    public bool IsFacebookUser { get; set; } 
}