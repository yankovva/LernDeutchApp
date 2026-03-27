using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.UserLessonProgress;

using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Course;

namespace LerningApp.Services.Data.CourseServices;

public class CourseQueryService( IRepository<Course, Guid> courseRepository,
    IRepository<UserCourse, object> userCourseRepository,
    IUserLessonProgressService userLessonProgressService) : ICourseQueryService
{
    public async Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(string? userId)
    {
        Guid? userGuidId = Guid.TryParse(userId, out var parsedId)
            ? parsedId
            : null;
        
        IEnumerable<CourseIndexViewModel> courses = await courseRepository
            .GetAllAttached()
            .AsNoTracking()
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CourseIndexViewModel
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                LessonsCount = c.LessonsForCourse.Count,
                CourseLevel = c.Level.Name,
                IsActive = c.IsPublished,
                EnrolledCount = c.CourseParticipants.Count,
                Price = c.Price,
                IsEnrolled = userId != null && c.CourseParticipants
                    .Any(cp => cp.UserId == userGuidId),
            })
            .ToListAsync();

        return courses;
    }
    
     public async Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResultT<CourseDetailsViewModel>.Fail((InvalidCourseIdMessage));
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(course => course.Level)
            .Include(course => course.LessonsForCourse)
            .ThenInclude(lesson => lesson.VocabularyCards).Include(course => course.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResultT<CourseDetailsViewModel>.Fail((CourseNotFoundMessage));
        }
        
        CourseDetailsViewModel model = new CourseDetailsViewModel()
        {
            Id = course.Id.ToString(),
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            LevelName = course.Level.Name,
            TotalWordsInCourse = course.LessonsForCourse.Select(l => l.VocabularyCards.Count).Sum(),
            PublisherId = course.PublisherId.ToString(),
            IsActive = course.IsPublished,
            CourseLessons = course.LessonsForCourse
                .OrderBy(l =>l.OrderIndex)
                .Select(cl => new CourseLessonsViewModel()
                {
                    LessinId = cl.Id.ToString(),
                    LessonName = cl.Name,
                    WordsInLesson = cl.VocabularyCards.Count(),
                    LessonTarget = cl.Target
                })
                .ToList()
        };
       
        if (Guid.TryParse(userId, out var userGuidId))
        {
            model.IsEnrolled = await userCourseRepository
                .GetAllAttached()
                .AnyAsync(uc => uc.UserId == userGuidId && uc.CourseId == courseId);
        }
        
        if (model.IsEnrolled)
        {
            var progressResult = await userLessonProgressService.GetCourseProgressPercent(courseId, userGuidId!);
            if (progressResult.Result)
            {
                model.ProgressPercentage = progressResult.Data;
            }
            else
            {
                model.ProgressPercentage = 0;
            }
            
            foreach (var lesson in model.CourseLessons)
            {
                var result = await userLessonProgressService
                    .GetUserLessonProgress(Guid.Parse(lesson.LessinId), userId);
                
                if (result.Result)
                {
                    lesson.UserLessonProgress = result.Data;
                }
                else
                {
                    lesson.UserLessonProgress = new IndexUserLessonProgressViewModel();
                }
            }
        }
        
        return ServiceResultT<CourseDetailsViewModel>.Success(model);
    }
     
    public async Task<List<CourseOptionsViewModel>> GetCourseOptionsAsync()
    {
        var courses = await courseRepository
            .GetAllAttached()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CourseOptionsViewModel
            {
                Id = c.Id.ToString(), 
                Name = c.Name
            })
            .ToListAsync();
        
        return courses;
    }
}