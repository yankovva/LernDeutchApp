using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

[Authorize]
public class ProfileController(UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IRepository<ApplicationUser, Guid> userRepository) : BaseController
{
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(this.User.GetUserId()!);
        var currentUser = await userRepository
            .GetAllAttached()
            .Include(u => u.Teacher)
            .Include(u => u.UserLessonsProgresses)
            .Include(u => u.UserCourses)
            .ThenInclude(uc => uc.Course)
            .ThenInclude(c => c.Level)
            .FirstOrDefaultAsync(u => u.Id == userId);

        
        bool isAdmin = await userManager.IsInRoleAsync(currentUser!, "Admin");

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
        
        return View(model);
    }
}