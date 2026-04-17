using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.LessonInterfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.ApplicationConstants;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data.LessonServices;

public class LessonCourseAssignmentService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<Course, Guid> courseRepository,
    ITeacherService teacherService,
    IRepository<UserLessonProgress, Guid> lessonProgressRepository,
    IRepository<UserCourse, object> userCourseRepository,
    UserManager<ApplicationUser> userManager) : ILessonCourseAssignmentService
{
    public async Task<ServiceResultT<AddLessonToCourseViewModel>> GetAddLessonToCourseByIdAsync(string id,string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail(InvalidLessonIdMessage, ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);

        if (lesson == null) 
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        var teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        AddLessonToCourseViewModel model = new AddLessonToCourseViewModel()
        {
            LessonId = lesson.Id.ToString(),
            LessonName = lesson.Name,
            SelectedCourseId = lesson.CourseId?.ToString().ToLower(),
            Courses = await courseRepository
                .GetAllAttached()
                .Select(c => new CourseCheckBoxItemInputModel
                {
                    CourseId = c.Id.ToString().ToLower(),
                    CourseName = c.Name,
                })
                .ToListAsync(),
        };
        return ServiceResultT<AddLessonToCourseViewModel>.Success(model);
    }

    public async Task<ServiceResult> AddLessonToCourseAsync(AddLessonToCourseViewModel model, string userId)
    {
        if (string.IsNullOrEmpty(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage, ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);

        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        var teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
       
        //TODO: Delete UserLessonProgress records for enrolled users when a lesson is removed from a course (decide: hard delete vs soft delete).
        if (string.IsNullOrWhiteSpace(model.SelectedCourseId))
        {
            lesson.CourseId = null;
            await lessonRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }
           
        if (string.IsNullOrEmpty(model.SelectedCourseId) || !Guid.TryParse(model.SelectedCourseId, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage, ServiceErrorType.Validation,nameof(model.SelectedCourseId));
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == courseId);    
        
        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage,ServiceErrorType.Validation,nameof(model.SelectedCourseId));
        }
        
        var participantsInCourseIds = await userCourseRepository
            .GetAllAttached()
            .Where(uc => uc.CourseId == courseId)
            .Select(uc=> uc.UserId)
            .ToListAsync();
        
        var userLessonProgressIds = await lessonProgressRepository
            .GetAllAttached()
            .Where(ul => ul.LessonId == lessonId)
            .Select(ul => ul.UserId)
            .ToListAsync();

        foreach (var participantId in participantsInCourseIds)
        {
            if (!userLessonProgressIds.Contains(participantId))
            {
                UserLessonProgress userLessonProgress = new UserLessonProgress()
                {
                    LessonId = lessonId,
                    UserId = participantId,
                    IsCompleted = false,
                    IsUnlocked = false
                };

                lessonProgressRepository.Add(userLessonProgress);
            }
        }
        
        lesson.CourseId = courseId;
        await lessonRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }
}