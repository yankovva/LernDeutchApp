using Microsoft.AspNetCore.Identity;

namespace LerningApp.Data.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        this.Id = Guid.NewGuid();
    }
    
    public virtual IEnumerable<UserCourse> UserCourses { get; set; } = new HashSet<UserCourse>();

}