using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.LessonInterfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Lesson;


namespace LerningApp.Services.Data.LessonServices;

public class LessonCommandService(IRepository<Lesson, Guid> lessonRepository,
    ITeacherService teacherService,
    IRepository<Course, Guid> courseRepository) : ILessonCommandService
{
     //TODO add logic for the order of the lessons in a course
    public async Task<ServiceResult> AddLessonAsync(AddLessonInputModel model, string userId)
    { 
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        Guid courseId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(model.CourseId))
        {
            if (!Guid.TryParse(model.CourseId, out  courseId))
            {
                return ServiceResult.Fail(InvalidCourseIdMessage, nameof(model.CourseId));
            }
            
            Course? course = await courseRepository
                .GetByIdAsync(courseId);
            
            if (course == null)
            {
                return ServiceResult.Fail(CourseNotFoundMessage, nameof(model.CourseId));
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
            return ServiceResultT<LessonEditInputModel>.Fail(InvalidLessonIdMessage);
        }

        Lesson? lesson = await lessonRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(LessonNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(AccessDeniedMessage);
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
            return ServiceResult.Fail(InvalidLessonIdMessage);
        }

        Lesson? lessonToChange = await lessonRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lessonToChange == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage);
        }

        Guid? courseId = null;
        if (!string.IsNullOrWhiteSpace(model.CourseId))
        {
            if (!Guid.TryParse(model.CourseId, out var parsedCourseId))
                return ServiceResult.Fail(InvalidCourseIdMessage);

            var course = await courseRepository.GetAllAttached()
                .FirstOrDefaultAsync(c => c.Id == parsedCourseId);
               
            if (course == null)
                return ServiceResult.Fail(CourseNotFoundMessage);

            courseId = parsedCourseId;
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || teacherId != lessonToChange.PublisherId)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(AccessDeniedMessage);
        }

        lessonToChange.Name = model.Name;
        lessonToChange.Content = model.Content;
        lessonToChange.CourseId = courseId;
        lessonToChange.Target = model.Target;
       
        await lessonRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
}