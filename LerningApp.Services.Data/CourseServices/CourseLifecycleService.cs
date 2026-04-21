using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.ApplicationConstants;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data.CourseServices;

public class CourseLifecycleService( IRepository<Course, Guid> courseRepository,
    ITeacherService teacherService,
    UserManager<ApplicationUser> userManager) : ICourseLifecycleService
{
    public async Task<ServiceResult> DeactivateCourseAsync(string id,string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage, ServiceErrorType.NotFound);
        }

        var course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || course.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        course.Status = CourseStatus.Inactive;
        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RestoreCourseAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage, ServiceErrorType.NotFound);
        }

        var course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || course.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        course.Status = CourseStatus.Published;
        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
    public async Task<ServiceResult> SoftDeleteCourseAsync(string id, string userId)
    {
        if (!Guid.TryParse(id, out var courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage, ServiceErrorType.NotFound);
        }

        var course = await courseRepository
            .GetAllAttached()
            .Include(c => c.LessonsForCourse)
            .Include(c => c.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || course.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        course.Status = CourseStatus.Deleted;

        foreach (var lesson in course.LessonsForCourse)
        {
            lesson.IsDeleted = true;
        }

        foreach (var uc in course.CourseParticipants)
        {
            uc.IsDeleted = true;
        }

        await courseRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
}