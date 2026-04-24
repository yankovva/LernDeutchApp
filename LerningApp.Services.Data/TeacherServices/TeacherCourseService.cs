using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces.TeacherInterfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Teacher;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Course;
namespace LerningApp.Services.Data.TeacherServices;

public class TeacherCourseService(IRepository<Course, Guid> courseRepository) : ITeacherCourseService
{
    public async Task<IEnumerable<CourseTeacherIndexViewModel>> GetAllCorsesAsync()
    {
        var courses = await courseRepository
            .GetAllAttached()
            .Include(l => l.Level)
            .Include(cp => cp.CourseParticipants)
            .Select(c => new CourseTeacherIndexViewModel
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                LevelName = c.Level.Name,
                Price = c.Price,
                IsPublished = c.Status == Enums.CourseStatus.Published,
                EnrolledStudentsCount = c.CourseParticipants.Count
            }).ToListAsync();

        return courses;
    }

    public async Task<ServiceResultT<CourseManageViewModel>> GetCourseManageByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResultT<CourseManageViewModel>.Fail(InvalidCourseIdMessage, Enums.ServiceErrorType.NotFound);
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(c => c.Level)
            .Include(c => c.CourseParticipants)
            .Include(c => c.LessonsForCourse)
            .ThenInclude(l => l.VocabularyCards)
            .Include(c => c.LessonsForCourse)
            .ThenInclude(l => l.ListeningExercises)
            .Include(c => c.LessonsForCourse)
            .ThenInclude(l => l.MultipleChoiceExercises)
            .Include(c => c.LessonsForCourse)
            .ThenInclude(l => l.TranslationExercises)
            .FirstOrDefaultAsync(c => c.Id == courseId);


        if (course == null)
        {
            return ServiceResultT<CourseManageViewModel>.Fail(CourseNotFoundMessage, Enums.ServiceErrorType.NotFound);
        }

        var totalWords = course
            .LessonsForCourse
            .Select(l => l.VocabularyCards)
            .Count();
        
        var totalExTranslationCount = course.LessonsForCourse
            .Select(l => l.TranslationExercises)
            .Count();
        
        var totalExListeningCount = course.LessonsForCourse
            .Select(l => l.ListeningExercises)
            .Count();
        var totalExMultiplegCount = course.LessonsForCourse
            .Select(l => l.MultipleChoiceExercises)
            .Count();
        
        var model = new CourseManageViewModel
        {
            Id = courseId.ToString(),
            Name = course.Name,
            LevelName = course.Level.Name,
            Price = course.Price,
            EnrolledStudentsCount = course.CourseParticipants.Count,
            Description = course.Description,
            Status = course.Status,
            CreatedAt = course.CreatedAt.ToString("dd/MM/yyyy"),
            TotalWordsCount = totalWords,
            TotalExercisesCount = totalExTranslationCount + totalExListeningCount + totalExMultiplegCount,
            LessonsCount = course.LessonsForCourse.Count,
            Lessons = course.LessonsForCourse
                .Select(l => new CourseManageLessonViewModel()
                {
                    Id = l.Id.ToString(),
                    Name = l.Name,
                    Target = l.Target,
                    OrderIndex = l.OrderIndex
                }).ToList()
        };
        
        return ServiceResultT<CourseManageViewModel>.Success(model);
    }
}