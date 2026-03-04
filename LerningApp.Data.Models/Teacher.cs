namespace LerningApp.Data.Models;

public class Teacher
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public bool IsApproved { get; set; }
    
    public string Qualification { get; set; } = null!;
    
    public string? ProfileImage { get; set; }
    
    public string? Biography { get; set; }

    public Guid UserId { get; set; } 
    
    public ApplicationUser User { get; set; } = null!;
}