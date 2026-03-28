namespace LerningApp.Web.ViewModels.Admin.User;

public class AdminUserIndexViewModel
{
    public string Id { get; set; } = null!;
    public string? Email { get; set; } 
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string UserName { get; set; } = null!;
    public string RegisteredOn { get; set; } = null!;
    public bool IsFacebookUser { get; set; } 
    public IList<string> Roles { get; set; } = new List<string>();
}