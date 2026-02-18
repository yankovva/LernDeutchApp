using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.LessonSection;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data;

public class LessonService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<LessonSection, Guid> lessonSectionRepository,
    IRepository<Course, Guid> courseRepository) : ILessonService
{
    public async Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync()
    {
        IEnumerable<LessonIndexViewModel> lessons =  await lessonRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(l => l.Course) 
            .OrderBy(l => l.Name)
            .Select(l => new LessonIndexViewModel
            {
                Id = l.Id.ToString(),
                Name = l.Name,
                CourseId = l.CourseId.ToString(),
                CourseName = l.Course != null ? l.Course.Name : null,
                LevelName = l.Course.Level != null ? l.Course.Level.Name : null, 
                CreatedAt = l.CreatedAt.ToString("dd.MM.yyyy"),
            })
            .ToListAsync();
        
        return lessons;
    }

    public async Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResultT<LessonContentViewModel>.Fail("Невалиден урок.");
        }
        
        Lesson? lesson = await lessonRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(l => l.Course)
            .Include(lesson => lesson.VocabularyCards)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return ServiceResultT<LessonContentViewModel>.Fail("Урокът не е намерен.");
        }

        LessonContentViewModel model = new LessonContentViewModel()
        {
            Id = lesson.Id.ToString(),
            Name = lesson.Name,
            CourseId = lesson.CourseId.ToString(),
            Content = lesson.Content,
            WordCount = lesson.VocabularyCards.Count(),
            OrderIndex = lesson.OrderIndex,
            CourseName = lesson.Course != null ? lesson.Course.Name : "No course found.",
            Target = lesson.Target,
            LessonSections = await lessonSectionRepository
                .GetAllAttached()
                .Where(ls => ls.LessonId == lessonId)
                .OrderBy(ls => ls.OrderIndex)
                .Select(ls => new LessonSectionViewModel()
                {
                    Type = ls.Type,
                    OrderIndex = ls.OrderIndex,
                    Content = ls.Content,
                })
                .ToListAsync()
        };
        
        return ServiceResultT<LessonContentViewModel>.Success(model);
    }

    public async Task<ServiceResultT<AddLessonToCourseViewModel>> GetAddLessonToCourseByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail("Невалиден урок.");
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);

        if (lesson == null) 
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail("Урокът не е намерен.");
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
                .ToListAsync()
        };
        return ServiceResultT<AddLessonToCourseViewModel>.Success(model);
    }

    public async Task<ServiceResult> AddLessonToCourseAsync(AddLessonToCourseViewModel model)
    {
        if (string.IsNullOrEmpty(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return ServiceResult.Fail("Урокът не е намерен.");
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);

        if (lesson == null)
        {
            return ServiceResult.Fail("Урокът не е намерен.");
        }
        
        if (string.IsNullOrWhiteSpace(model.SelectedCourseId))
        {
            lesson.CourseId = null;
            await lessonRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }
           
        if (string.IsNullOrEmpty(model.SelectedCourseId) || !Guid.TryParse(model.SelectedCourseId, out Guid courseId))
        {
            return ServiceResult.Fail("Невалиден Курс.", nameof(model.SelectedCourseId));
        }

        Course? course = await courseRepository
            .GetByIdAsync(courseId);
        
        if (course == null)
        {
            return ServiceResult.Fail("Невалиден Курс.",nameof(model.SelectedCourseId));
        }

        lesson.CourseId = courseId;

        await lessonRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }
}