using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using Microsoft.EntityFrameworkCore;
namespace LerningApp.Services.Data;

public class CourseService(
    IRepository<Course, Guid> courseRepository,
    IRepository<Level, Guid> levelRepository,
    IRepository<UserCourse, object> userCourseRepository) : ICourseService
{
    public async Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(Guid? userId)
    {
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
                IsEnrolled = userId != null && c.CourseParticipants
                    .Any(cp => cp.UserId == userId),
            })
            .ToListAsync();

        return courses;
    }

    public async Task<ServiceResult> AddCourseAsync(AddCourseViewModel model, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail("Невалидно ниво.");
        }

        Level? level = await levelRepository
            .GetByIdAsync(levelId);

        if (level == null)
        {
            return ServiceResult.Fail("Невалидно ниво.");
        }

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            LevelId = levelId,
            IsPublished = true,
            CreatedAt = DateTime.Now,
            PublisherId = userId
        };

        await courseRepository.AddAsync(course);

        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResultT<CourseDetailsViewModel>.Fail(("Невалиден курс."));
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(course => course.Level)
            .Include(course => course.LessonsForCourse)
            .ThenInclude(lesson => lesson.VocabularyCards).Include(course => course.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResultT<CourseDetailsViewModel>.Fail(("Курсът не е намерен."));
        }

        CourseDetailsViewModel model = new CourseDetailsViewModel()
        {
            Id = course.Id.ToString(),
            Name = course.Name,
            Description = course.Description,
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

    public async Task<ServiceResultT<CourseEditViewModel>> GetCourseEditByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResultT<CourseEditViewModel>.Fail("Невалиден курс.");
        }

        Course? course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResultT<CourseEditViewModel>.Fail("Курсът не е намерен.");
        }

        CourseEditViewModel model = new CourseEditViewModel()
        {
            Id = courseId.ToString(),
            Name = course.Name,
            Description = course.Description,
            LevelId = course.LevelId.ToString()
        };

        return ServiceResultT<CourseEditViewModel>.Success(model);
    }

    public async Task<ServiceResult> PostEditCourseAsync(CourseEditViewModel model, string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail("Невалиден курс.", nameof(id));
        }

        Course? courseToChange = await courseRepository
            .GetByIdAsync(courseId);

        if (courseToChange == null)
        {
            return ServiceResult.Fail("Курсът не е намерен.", nameof(id));
        }

        if (string.IsNullOrEmpty(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail("Невалидно ниво.", nameof(model.LevelId));
        }

        var levelExists = await levelRepository
            .GetByIdAsync(levelId);

        if (levelExists == null)
        {
            return ServiceResult.Fail("Избраното ниво не съществува.", nameof(model.LevelId));
        }

        courseToChange.Name = model.Name;
        courseToChange.Description = model.Description;
        courseToChange.LevelId = levelId;

        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeactivateCourseAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail("Невалиден курс.");
        }

        var course = await courseRepository
           .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResult.Fail("Невалиден курс.");
        }

        course.IsPublished = false;
        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RestoreCourseAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail("Курсът не е намерен.");
        }

        var course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResult.Fail("Курсът не е намерен.");
        }

        course.IsPublished = true;
        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> EnrollInCourseAsync(string id, Guid userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail("Невалиден курс.");
        }

        Course? course = await courseRepository
            .FirstorDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResult.Fail("Курсът не е намерен.");
        }
        
        bool alreadyEnrolled = await userCourseRepository
            .GetAllAttached()
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (alreadyEnrolled)
        {
            return ServiceResult.Fail("Вече сте записани за този курс.");
        }

        UserCourse newUserCourse = new UserCourse
        {
            UserId = userId,
            CourseId = courseId,
            StartedAt = DateTime.UtcNow
        };

        await userCourseRepository.AddAsync(newUserCourse);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SoftDeleteCourseAsync(string id)
    {
        if (!Guid.TryParse(id, out var courseId))
        {
            return ServiceResult.Fail("Невалиден курс.");
        }

        var course = await courseRepository
            .GetAllAttached()
            .Include(c => c.LessonsForCourse)
            .Include(c => c.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return ServiceResult.Fail("Курсът не е намерен.");
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