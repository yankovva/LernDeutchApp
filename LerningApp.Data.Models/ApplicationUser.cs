using LerningApp.Common;
using Microsoft.AspNetCore.Identity;
using static LerningApp.Common.Enums;

namespace LerningApp.Data.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        this.Id = Guid.NewGuid();
    }

    public Teacher? Teacher { get; set; }

    public TranslationLanguage NativeLanguage { get; set; } = TranslationLanguage.Bg;
    public virtual ICollection<UserCourse> UserCourses { get; set; } = new HashSet<UserCourse>();
    
}