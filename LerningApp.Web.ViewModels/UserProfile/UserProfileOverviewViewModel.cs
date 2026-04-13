using static LerningApp.Common.Enums;

namespace LerningApp.Web.ViewModels.UserProfile;

public class UserProfileOverviewViewModel
{
    public string FullName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public TranslationLanguage NativeLanguage { get; set; }
    public bool IsTeacher { get; set; }
    public bool IsAdmin { get; set; }
    public int EnrolledCoursesCount { get; set; }
    public int CompletedCoursesCount { get; set; }
    public int LearnedWordsCount { get; set; }
    public int CompletedLessonsCount { get; set; }

    public IEnumerable<UserProfileCourseRowViewModel> EnrolledCourses { get; set; }
        = new List<UserProfileCourseRowViewModel>();
}