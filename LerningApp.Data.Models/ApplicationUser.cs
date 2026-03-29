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

    [PersonalData]
    public string? FirstName { get; set; }
    
    [PersonalData]
    public string? LastName { get; set; }
    
    [PersonalData]
    public string? ProfileImage { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime DeletedAt { get; set; }

    public Teacher? Teacher { get; set; }

    public TranslationLanguage NativeLanguage { get; set; } = TranslationLanguage.Bg;
    public virtual ICollection<UserCourse> UserCourses { get; set; } = new HashSet<UserCourse>();
    public virtual ICollection<UserLessonProgress> UserLessonsProgresses { get; set; } = new HashSet<UserLessonProgress>();
    public virtual ICollection<UserExerciseProgress> UserExerciseProgresses { get; set; } = new HashSet<UserExerciseProgress>();
}