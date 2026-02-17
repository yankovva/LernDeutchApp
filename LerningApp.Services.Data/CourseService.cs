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

    public async Task<ServiceResult> AddCourseAsync(AddCourseViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail("Невалидно ниво.");
        }

        Level? level = await levelRepository
            .GetAllAttached()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == levelId);

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
            .AsNoTracking()
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

        if (userId != null)
        {
            model.IsEnrolled = await userCourseRepository
                .GetAllAttached()
                .AnyAsync(uc => uc.UserId == Guid.Parse(userId) && uc.CourseId == courseId);
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
            .GetAllAttached()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId);

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

        Course? courseToChange = courseRepository
            .GetAllAttached()
            .FirstOrDefault(c => c.Id == courseId);

        if (courseToChange == null)
        {
            return ServiceResult.Fail("Курсът не е намерен.", nameof(id));
        }

        if (string.IsNullOrEmpty(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail("Невалидно ниво.", nameof(model.LevelId));
        }

        bool levelExists = await levelRepository
            .GetAllAttached()
            .AnyAsync(l => l.Id == levelId);

        if (!levelExists)
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
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == courseId);

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
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == courseId);

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
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == courseId && c.IsPublished == true);

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
        await userCourseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}