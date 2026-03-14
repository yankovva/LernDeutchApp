using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using static LerningApp.Common.EntityErrorMessages.Level;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Course;


using Microsoft.EntityFrameworkCore;
namespace LerningApp.Services.Data;

public class CourseService(
    IRepository<Course, Guid> courseRepository,
    IRepository<Level, Guid> levelRepository,
    IRepository<UserCourse, object> userCourseRepository,
    ITeacherService teacherService,
    IUserLessonProgressService userLessonProgressService,
    IRepository<UserLessonProgress, Guid> userProgressRepository) : ICourseService
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
    

    public async Task<ServiceResult> AddCourseAsync(AddCourseViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail(InvalidLevelIdMessage);
        }

        Level? level = await levelRepository
            .GetByIdAsync(levelId);

        if (level == null)
        {
            return ServiceResult.Fail(LevelNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        
        if (teacherId == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            LevelId = levelId,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            PublisherId = teacherId.Value,
            Price = model.Price
        };

        await courseRepository.AddAsync(course);

        return ServiceResult.Success();
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
                .Select(cl => new CourseLessonsViewModel()
                {
                    LessinId = cl.Id.ToString(),
                    LessonName = cl.Name,
                    WordsInLesson = cl.VocabularyCards.Count(),
                    LessonTarget = cl.Target
                }).ToList()
        };
        
       
        if (Guid.TryParse(userId, out var userGuidId))
        {
            model.IsEnrolled = await userCourseRepository
                .GetAllAttached()
                .AnyAsync(uc => uc.UserId == userGuidId && uc.CourseId == courseId);
        }
        
        return ServiceResultT<CourseDetailsViewModel>.Success(model);
    }

    public async Task<ServiceResultT<CourseEditViewModel>> GetCourseEditByIdAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResultT<CourseEditViewModel>.Fail(InvalidCourseIdMessage);
        }

        Course? course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResultT<CourseEditViewModel>.Fail(CourseNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        
        if (teacherId == null || course.PublisherId != teacherId)
        {
            return ServiceResultT<CourseEditViewModel>.Fail(AccessDeniedMessage);
        }

        CourseEditViewModel model = new CourseEditViewModel()
        {
            Id = courseId.ToString(),
            Name = course.Name,
            Price = course.Price,
            Description = course.Description,
            LevelId = course.LevelId.ToString()
        };

        return ServiceResultT<CourseEditViewModel>.Success(model);
    }

    public async Task<ServiceResult> PostEditCourseAsync(CourseEditViewModel model, string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage, nameof(id));
        }

        Course? courseToChange = await courseRepository
            .GetByIdAsync(courseId);

        if (courseToChange == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage, nameof(id));
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || courseToChange.PublisherId != teacherId)
        {
            return ServiceResultT<CourseEditViewModel>.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrEmpty(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail(InvalidLevelIdMessage, nameof(model.LevelId));
        }

        var levelExists = await levelRepository
            .GetByIdAsync(levelId);

        if (levelExists == null)
        {
            return ServiceResult.Fail(LevelNotFoundMessage, nameof(model.LevelId));
        }

        courseToChange.Name = model.Name;
        courseToChange.Description = model.Description;
        courseToChange.LevelId = levelId;
        courseToChange.Price = model.Price;

        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

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

    public async Task<ServiceResult> EnrollInCourseAsync(string id, Guid userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage);
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(c => c.LessonsForCourse)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage);
        }
        
        bool alreadyEnrolled = await userCourseRepository
            .GetAllAttached()
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (alreadyEnrolled)
        {
            return ServiceResult.Fail(AlreadyEnrolled);
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

        await userCourseRepository.AddAsync(newUserCourse);
        await userProgressRepository.AddRangeAsync(progresses);
        await userProgressRepository.SaveChangesAsync();

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