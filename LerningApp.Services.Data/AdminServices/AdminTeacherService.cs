using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using LerningApp.Web.ViewModels.Admin.Teacher;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.Enums;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data.AdminServices;

public class AdminTeacherService(UserManager<ApplicationUser> userManager,
    IRepository<Teacher, Guid> teacherRepository,
    IFileService fileService) : IAdminTeacherService
{
    public async Task<IEnumerable<AdminTeacherIndexViewModel>> GetAllTeachersNotDeletedAsync()
    {
        var teachers = await teacherRepository
            .GetAllAttached()
            .Include(x => x.User)
            .Where(t => t.User.IsDeleted == false)
            .Select(t => new AdminTeacherIndexViewModel()
            {
                UserId = t.UserId.ToString(),
                TeacherId = t.Id.ToString(),
                Email = t.User.Email,
                FirstName = t.User.FirstName,
                LastName = t.User.LastName,
                HasPendingProfileChanges = t.HasProfileChangesPendingReview,
                Status = t.Status.ToString(),
                TeacherSince = t.TeacherSince.HasValue
                    ? t.TeacherSince.Value.ToString("MM/dd/yyyy")
                    : "Pending"
            })
            .ToListAsync();

        return teachers;
    }

    public async Task<ServiceResult> AddPendingTeacherAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.", ServiceErrorType.Conflict );
        }
        bool isTeacherRole = await userManager.IsInRoleAsync(user, TeacherRole);
        if (isTeacherRole && user.Teacher != null)
        {
            return ServiceResult.Fail("User is a teacher or has already been requested." , ServiceErrorType.Conflict);
        }

        var newTeacher = new Teacher()
        {
            UserId = Guid.Parse(id),
            Status = TeacherStatus.Draft
        };
            
        teacherRepository.Add(newTeacher);
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }

    private async Task<bool> AssignTeacherRole(ApplicationUser user, string userId)
    {
        var result = await userManager
            .AddToRoleAsync(user, TeacherRole);
        if (!result.Succeeded)
        {
            return false;
        }
        
        Guid userGuid = Guid.Parse(userId);
        var teacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == userGuid);
        
        if (teacher == null)
        {
            return false;
        }
        teacher.TeacherSince = DateTime.UtcNow;
        await teacherRepository.SaveChangesAsync();
        
        return true;
    }
    public async Task<ServiceResult> ApproveUserProfileChangesAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.", ServiceErrorType.Conflict);
        }
        
        //First request for teacher 
        bool isTeacher = await userManager
            .IsInRoleAsync(user, TeacherRole);
        if (!isTeacher)
        {
            bool roleAssigned = await AssignTeacherRole(user, userId);
            if (!roleAssigned)
            {
                return ServiceResult.Fail("Failed to assign teacher role.", ServiceErrorType.General);
            }
        }
        
        Guid userGuid = Guid.Parse(userId);
        var teacher = await teacherRepository
            .GetAllAttached()
            .Include(x => x.User)
            .FirstOrDefaultAsync(t => t.UserId == userGuid);
        
        if (teacher == null)
        {
            return ServiceResult.Fail("No teacher found.", ServiceErrorType.Conflict);
        }

        if (!string.IsNullOrWhiteSpace(teacher.PendingProfileImage))
        {
            if (!string.IsNullOrWhiteSpace(teacher.User.ProfileImage) &&
                teacher.User.ProfileImage != teacher.PendingProfileImage)
            {
                fileService.DeleteFile(teacher.User.ProfileImage);
            }
            teacher.User.ProfileImage = teacher.PendingProfileImage;
        }

        teacher.Qualification = teacher.PendingQualification;
        teacher.Biography = teacher.PendingBiography;
        teacher.User.FirstName = teacher.PendingFirstName;
        teacher.User.LastName = teacher.PendingLastName;
        teacher.User.PhoneNumber = teacher.PendingPhoneNumber;
        teacher.Status = TeacherStatus.Approved;

        teacher.PendingFirstName = null;
        teacher.PendingLastName = null;
        teacher.PendingPhoneNumber = null;
        teacher.PendingBiography = null;
        teacher.PendingQualification = null;
        teacher.PendingProfileImage = null;
        teacher.HasProfileChangesPendingReview = false;
        
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RejectTeacherChangesRequestAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.", ServiceErrorType.Conflict);
        }

        bool isTeacher = await userManager.IsInRoleAsync(user, TeacherRole);

        Guid userGuid = Guid.Parse(userId);
        var teacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == userGuid);

        if (teacher == null)
        {
            return ServiceResult.Fail("No teacher request found.", ServiceErrorType.Conflict);
        }

        if (!isTeacher)
        {
            teacher.Status = TeacherStatus.Rejected;
        }
        else
        {
            teacher.Status = TeacherStatus.Approved;
        }

        teacher.PendingFirstName = null;
        teacher.PendingLastName = null;
        teacher.PendingPhoneNumber = null;
        teacher.PendingBiography = null;
        teacher.PendingQualification = null;
        teacher.PendingProfileImage = null;
        teacher.HasProfileChangesPendingReview = false;

        await teacherRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }


    public async Task<ServiceResult> RemoveTeacherRoleAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.",ServiceErrorType.Conflict);
        }
        
        bool isTeacher = await userManager.IsInRoleAsync(user, TeacherRole);
        if (!isTeacher)
        {
            return ServiceResult.Fail("User not in role teacher.", ServiceErrorType.Conflict);
        }
        Guid parsedUserId = Guid.Parse(userId);
        
        var teacher = await teacherRepository
            .FirstorDefaultAsync(u => u.UserId == parsedUserId);
        
        if (teacher == null)
        {
            return ServiceResult.Fail("User not a teacher.", ServiceErrorType.Conflict);
        }
        
        var roleResult = await userManager.RemoveFromRoleAsync(user, TeacherRole);
        if (!roleResult.Succeeded)
        {
            return ServiceResult.Fail("Failed to remove teacher role.", ServiceErrorType.General);
        }
        teacher.PendingFirstName = null;
        teacher.PendingLastName = null;
        teacher.PendingPhoneNumber = null;
        teacher.PendingProfileImage = null;
        teacher.PendingBiography = null;
        teacher.PendingQualification = null;
        teacher.HasProfileChangesPendingReview = false;
        teacher.Status = TeacherStatus.Inactive;

        await teacherRepository.SaveChangesAsync();
       
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveTeacherRequestAsync(string teacherId)
    {
        Guid id = Guid.TryParse(teacherId, out Guid teacherGuid)
            ? teacherGuid
            : Guid.Empty;
        
        Teacher? teacher = await teacherRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(t => t.Id == teacherGuid && t.Status == TeacherStatus.PendingReview);
        if (teacher == null)
        {
            return ServiceResult.Fail("User does not have a pending teacher request.", ServiceErrorType.Conflict);
        }
        
        teacherRepository.DeleteByEntity(teacher);
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }

    public async  Task<ServiceResultT<AdminTeacherDetailsViewModel>> GetTeacherDetailsAsync(string teacherId)
    {
        Guid id = Guid.TryParse(teacherId, out Guid teacherGuid)
            ? teacherGuid
            : Guid.Empty;
        
        Teacher? teacher = await teacherRepository
            .GetAllAttached()
            .Include(t => t.ListeningExercises)
            .Include(t=>t.CreatedTranslationExercises)
            .Include(t=>t.CreatedMultipleChoiceExercises)
            .Include(t=>t.CreatedCourses)
            .Include(t =>t.CreatedLessons)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == teacherGuid);
        
        if (teacher == null)
        {
            return ServiceResultT<AdminTeacherDetailsViewModel>.Fail("User is not Teacher", ServiceErrorType.Conflict);
        }

        var result = new AdminTeacherDetailsViewModel()
        {
            UserId = teacher.UserId.ToString(),
            TeacherId = teacher.Id.ToString(),
            Email = teacher.User.Email,
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            UserName = teacher.User.UserName,
            PhoneNumber = teacher.User.PhoneNumber,
            Qualifications = teacher.Qualification,
            Status = teacher.Status.ToString(),
            Biography = teacher.Biography,
            TeacherSince = teacher.TeacherSince.HasValue
                ? teacher.TeacherSince.Value.ToString("MM/dd/yyyy")
                : "Pending",
            ProfileImage = teacher.User.ProfileImage,
            PendingFirstName = teacher.PendingFirstName,
            PendingLastName = teacher.PendingLastName,
            PendingPhoneNumber = teacher.PendingPhoneNumber,
            PendingBiography = teacher.PendingBiography,
            PendingProfileImage = teacher.PendingProfileImage,
            PendingQualifications = teacher.PendingQualification,
            HasPendingChanges = teacher.HasProfileChangesPendingReview,
            CreatedVocabularyCardsCount = 100,
            CreatedCoursesCount = teacher.CreatedCourses.Count,
            CreatedLessonsCount = teacher.CreatedLessons.Count,
            CreatedListeningExercisesCount = teacher.ListeningExercises.Count,
            CreatedTranslationExercisesCount = teacher.CreatedTranslationExercises.Count,
            CreatedMultipleChoiceExercisesCount = teacher.CreatedMultipleChoiceExercises.Count,
            TotalCreatedExercisesCount =  teacher.ListeningExercises.Count + teacher.CreatedTranslationExercises.Count + teacher.CreatedMultipleChoiceExercises.Count,
        };
        
        return ServiceResultT<AdminTeacherDetailsViewModel>.Success(result);
    }

    public async Task<ServiceResult> ReturnRemovedTeacher(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.", ServiceErrorType.NotFound);
        }
        Guid id = Guid.TryParse(userId, out Guid userGuid)
            ? userGuid
            : Guid.Empty;
        Teacher? userTeacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == userGuid);
        if (userTeacher == null)
        {
            return ServiceResult.Fail("No teacher found.", ServiceErrorType.NotFound);
        }
        
        bool isTeacher = await userManager.IsInRoleAsync(user, TeacherRole);
        if (!isTeacher)
        {
            var roleResult = await userManager.AddToRoleAsync(user, TeacherRole);
            if (!roleResult.Succeeded)
            {
                return ServiceResult.Fail("Failed to restore teacher role.", ServiceErrorType.General);
            }
        }

        userTeacher.Status = TeacherStatus.Draft;
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }
}