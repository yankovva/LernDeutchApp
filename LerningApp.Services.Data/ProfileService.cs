using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.UserProfile;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data;

public class ProfileService(UserManager<ApplicationUser> userManager,
    IRepository<ApplicationUser, Guid> userRepository) : IProfileService
{
    public async Task<UserProfileOverviewViewModel> IndexGetUserProfileOverviewModelAsync(Guid userId)
    {
        var currentUser = await userRepository
            .GetAllAttached()
            .Include(u => u.Teacher)
            .Include(u => u.UserLessonsProgresses)
            .Include(u => u.UserCourses)
            .ThenInclude(uc => uc.Course)
            .ThenInclude(c => c.Level)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        bool isAdmin = await userManager.IsInRoleAsync(currentUser!, AdminRole);

        List<UserProfileCourseRowViewModel> enrolledCourses = currentUser
            .UserCourses
            .Select(c => new UserProfileCourseRowViewModel()
            {
                Name = c.Course.Name,
                LevelName = c.Course.Level.Name,
                Status = c.CompletedAt != null ? "completed" : "not completed",
            })
            .ToList();
        
        var model = new UserProfileOverviewViewModel
        {
            FullName = $"{currentUser!.FirstName} {currentUser.LastName}",
            Email = currentUser.Email,
            UserName = currentUser.UserName,
            FirstName = currentUser.FirstName,
            LastName = currentUser.LastName,
            PhoneNumber = currentUser.PhoneNumber,
            ProfileImageUrl = currentUser.ProfileImage,
            NativeLanguage = currentUser.NativeLanguage,
            IsTeacher = currentUser.Teacher != null,
            IsAdmin = isAdmin,
            EnrolledCoursesCount = currentUser.UserCourses.Count,
            CompletedCoursesCount = currentUser.UserCourses.Count(c => c.CompletedAt != null),
            LearnedWordsCount = 1000,
            CompletedLessonsCount = currentUser.UserLessonsProgresses.Count(up => up.IsCompleted),
            EnrolledCourses = enrolledCourses
        };
        
        return model;
    }
}