using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;

using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.Enums;
namespace LerningApp.Services.Data.CourseServices;

public class CourseEnrollmentService(IRepository<Course, Guid> courseRepository,
    IRepository<UserCourse, object> userCourseRepository,
    IRepository<UserLessonProgress, Guid> userProgressRepository) : ICourseEnrollmentService
{
    public async Task<ServiceResult> EnrollInCourseAsync(string id, Guid userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage,ServiceErrorType.NotFound);
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(c => c.LessonsForCourse)
            .FirstOrDefaultAsync(c => c.Id == courseId && c.Status == CourseStatus.Published);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        bool alreadyEnrolled = await userCourseRepository
            .GetAllAttached()
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (alreadyEnrolled)
        {
            return ServiceResult.Fail(AlreadyEnrolled, ServiceErrorType.Conflict);
        }

        UserCourse newUserCourse = new UserCourse
        {
            UserId = userId,
            CourseId = courseId,
            StartedAt = DateTime.UtcNow
        };

        var lessons = course.LessonsForCourse
            .OrderBy(l => l.OrderIndex)
            .ToList();

        var firstLessonId = lessons.FirstOrDefault()?.Id;

        var progresses = lessons
            .Select(l => new UserLessonProgress
            {
                UserId = userId,
                LessonId = l.Id,
                IsCompleted = false,
                IsUnlocked = firstLessonId != null && l.Id == firstLessonId
            }).ToList();

        userCourseRepository.Add(newUserCourse);
        userProgressRepository.AddRange(progresses);
        await userProgressRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}