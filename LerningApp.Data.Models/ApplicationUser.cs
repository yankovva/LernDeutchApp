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
    public virtual ICollection<Course> CreatedCourses { get; set; } = new HashSet<Course>();
    public virtual ICollection<Lesson> CreatedLessons { get; set; } = new HashSet<Lesson>();
    public virtual ICollection<MultipleChoiceExercise> CreatedMultipleChoiceExercises { get; set; } = new HashSet<MultipleChoiceExercise>();
    public virtual ICollection<TranslationExercise> CreatedTranslationExercises { get; set; } = new HashSet<TranslationExercise>();

}