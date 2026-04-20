using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Teacher;
using LerningApp.Web.ViewModels.UserProfile;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.ApplicationConstants;
using static LerningApp.Common.Enums;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.File;

namespace LerningApp.Services.Data;

public class ProfileService(UserManager<ApplicationUser> userManager,
    IRepository<ApplicationUser, Guid> userRepository,
    IRepository<Teacher,Guid> teacherRepository,
    IFileService fileService) : IProfileService
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

    public async Task<ServiceResultT<ProfileIndexViewModel>> GetTeacherProfileIndexViewModelAsync(Guid userId)
    {
        var teacher = await teacherRepository
            .GetAllAttached()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId);
        
        if (teacher == null)
        {
            return ServiceResultT<ProfileIndexViewModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        var model = new ProfileIndexViewModel
        {
            TeacherId = teacher.Id.ToString(),
            UserId = teacher.UserId.ToString(),
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            UserName = teacher.User.UserName,
            Email = teacher.User.Email,
            PhoneNumber = teacher.User.PhoneNumber,
            ProfileImage = teacher.User.ProfileImage,
            Status = teacher.Status.ToString(),
            TeacherSince = teacher.TeacherSince.HasValue
                ? teacher.TeacherSince.Value.ToString("MM/dd/yyyy")
                : string.Empty,    
            Biography = teacher.Biography,
            Qualifications = teacher.Qualification
        };
        return ServiceResultT<ProfileIndexViewModel>.Success(model);
    }

    public async Task<ServiceResultT<ProfileEditViewModel>> GetTeacherProfileEditViewModelAsync(Guid userId)
    {
        var teacher = await teacherRepository
            .GetAllAttached()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return ServiceResultT<ProfileEditViewModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        var model = new ProfileEditViewModel
        {
            TeacherId = teacher.Id.ToString(),
            UserId = teacher.UserId.ToString(),
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            PhoneNumber = teacher.User.PhoneNumber,
            ProfileImage = teacher.User.ProfileImage,
            Biography = teacher.Biography,
            Qualifications = teacher.Qualification
        };
        return ServiceResultT<ProfileEditViewModel>.Success(model);
    }

    public async Task<ServiceResult> PostTeacherProfileEditAsync(Guid userId, ProfileEditViewModel model)
    {
        
        if (!Guid.TryParse(model.TeacherId, out Guid teacherId))
        {
            return ServiceResult.Fail(InvalidOperationMessage, ServiceErrorType.Validation);
        }

        var user = await userManager.Users
            .Include(u => u.Teacher)
            .SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Teacher == null || user.Teacher.Id != teacherId)
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        var teacher = await teacherRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        string imagePath = string.Empty;

        if (model.Image?.Length > 0)
        {
            if (!fileService.IsFileValid(model.Image, AllowedImageExtensions, MaxFileSize))
            {
                 return ServiceResult.Fail(InvalidFileMessage, ServiceErrorType.Validation);
            }

            string extension = Path.GetExtension(model.Image.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            try
            {
                imagePath = await fileService
                    .UploadFileAsync(model.Image,
                        DefaultTеacherProfileImageDirectoryPath,
                        uniqueFileName);
            }
            catch (Exception e)
            {
                return ServiceResult.Fail(e.Message, ServiceErrorType.Validation);
            }
        }
        
        teacher.PendingFirstName = model.FirstName;
        teacher.PendingLastName = model.LastName;
        teacher.PendingPhoneNumber = model.PhoneNumber;
        teacher.PendingBiography = model.Biography;
        teacher.PendingQualification = model.Qualifications;
        
        if (!string.IsNullOrEmpty(imagePath))
        {
            teacher.PendingProfileImage = imagePath;
        }
        
        teacher.HasProfileChangesPendingReview = true;
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }
}