using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.LessonInterfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.ApplicationConstants;
using static LerningApp.Common.Enums;


namespace LerningApp.Services.Data.LessonServices;

public class LessonCommandService(IRepository<Lesson, Guid> lessonRepository,
    ITeacherService teacherService,
    IRepository<Course, Guid> courseRepository,
    UserManager<ApplicationUser> userManager) : ILessonCommandService
{
     //TODO add logic for the order of the lessons in a course
    public async Task<ServiceResult> AddLessonAsync(AddLessonInputModel model, string userId)
    { 
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }
        
        Guid courseId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(model.CourseId))
        {
            if (!Guid.TryParse(model.CourseId, out  courseId))
            {
                return ServiceResult.Fail(InvalidCourseIdMessage, ServiceErrorType.Validation,nameof(model.CourseId));
            }
            
            Course? course = await courseRepository
                .GetByIdAsync(courseId);
            
            if (course == null)
            {
                return ServiceResult.Fail(CourseNotFoundMessage, ServiceErrorType.Validation,nameof(model.CourseId));
            }
        }
        
        Lesson lesson = new Lesson
        {
            Name = model.Name,
            Content = model.Content,
            CourseId = courseId == Guid.Empty ? null : courseId,
            CreatedAt = DateTime.UtcNow,
            PublisherId = teacherId.Value,
            Target = model.Target,
        };
        
        lessonRepository.Add(lesson);
        await lessonRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<LessonEditInputModel>> GetLessonEditInputModelAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResultT<LessonEditInputModel>.Fail(InvalidLessonIdMessage, ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<LessonEditInputModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        var model = new LessonEditInputModel
        {
            Id = lesson.Id.ToString(),
            Name = lesson.Name,
            Content = lesson.Content,
            OrderIndex = lesson.OrderIndex,
            CourseId = lesson.CourseId?.ToString(),
            Target = lesson.Target,
            Courses = new List <CourseOptionsViewModel>{}
        };
        
        return ServiceResultT<LessonEditInputModel>.Success(model);
    }

    public async Task<ServiceResult> PostLessonEditInputModelAsync(LessonEditInputModel model, string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage, ServiceErrorType.NotFound);
        }

        Lesson? lessonToChange = await lessonRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lessonToChange == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }

        Guid? courseId = null;
        if (!string.IsNullOrWhiteSpace(model.CourseId))
        {
            if (!Guid.TryParse(model.CourseId, out var parsedCourseId))
            {
                return ServiceResult.Fail(InvalidCourseIdMessage, ServiceErrorType.Validation,nameof(model.CourseId));
            }

            var course = await courseRepository.GetAllAttached()
                .FirstOrDefaultAsync(c => c.Id == parsedCourseId);
               
            if (course == null)
                return ServiceResult.Fail(CourseNotFoundMessage, ServiceErrorType.NotFound);

            courseId = parsedCourseId;
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lessonToChange.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        lessonToChange.Name = model.Name;
        lessonToChange.Content = model.Content;
        lessonToChange.CourseId = courseId;
        lessonToChange.Target = model.Target;
       
        await lessonRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
}