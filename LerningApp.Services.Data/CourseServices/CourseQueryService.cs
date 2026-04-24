using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Teacher;
using LerningApp.Web.ViewModels.UserLessonProgress;

using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.Enums;

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
                IsActive = c.Status == CourseStatus.Published,
                EnrolledCount = c.CourseParticipants.Count,
                Price = c.Price,
                IsEnrolled = userId != null && c.CourseParticipants
                    .Any(cp => cp.UserId == userGuidId),
            })
            .ToListAsync();

        return courses;
    }
    
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
    
     public async Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResultT<CourseDetailsViewModel>.Fail(InvalidCourseIdMessage, ServiceErrorType.NotFound );
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(course => course.Level)
            .Include(course => course.LessonsForCourse)
            .ThenInclude(lesson => lesson.VocabularyCards)
            .Include(course => course.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResultT<CourseDetailsViewModel>.Fail(CourseNotFoundMessage, ServiceErrorType.NotFound);
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
            Status = course.Status,
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
     
    public async Task<List<CourseOptionsViewModel>> GetAssignableCourseOptionsAsync()
    {
        var courses = await courseRepository
            .GetAllAttached()
            .Where(c => c.Status == CourseStatus.Draft ||
                        c.Status == CourseStatus.Published)
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

        var totalWords = course.LessonsForCourse
            .Sum(l => l.VocabularyCards.Count);
        
        var totalExTranslationCount = course.LessonsForCourse
            .Sum(l => l.TranslationExercises.Count);
        
        var totalExListeningCount = course.LessonsForCourse
            .Sum(l => l.ListeningExercises.Count);
        
        var totalExMultipleCount = course.LessonsForCourse
            .Sum(l => l.MultipleChoiceExercises.Count);

        
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
            TotalExercisesCount = totalExTranslationCount + totalExListeningCount + totalExMultipleCount,
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