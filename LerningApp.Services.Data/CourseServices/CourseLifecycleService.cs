using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;

using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Course;

namespace LerningApp.Services.Data.CourseServices;

public class CourseLifecycleService( IRepository<Course, Guid> courseRepository,
    ITeacherService teacherService) : ICourseLifecycleService
{
    public async Task<ServiceResult> DeactivateCourseAsync(string id,string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage);
        }

        var course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || course.PublisherId != teacherId)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        course.IsPublished = false;
        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RestoreCourseAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage);
        }

        var course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || course.PublisherId != teacherId)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        course.IsPublished = true;
        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
    public async Task<ServiceResult> SoftDeleteCourseAsync(string id, string userId)
    {
        if (!Guid.TryParse(id, out var courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage);
        }

        var course = await courseRepository
            .GetAllAttached()
            .Include(c => c.LessonsForCourse)
            .Include(c => c.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || course.PublisherId != teacherId)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        course.IsDeleted = true;

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