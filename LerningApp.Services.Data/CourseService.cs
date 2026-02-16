using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using Microsoft.EntityFrameworkCore;
namespace LerningApp.Services.Data;

public class CourseService(IRepository<Course, Guid> courseRepository,
    IRepository<Level, Guid> levelRepository) : ICourseService
{
    public async Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(Guid? userId)
    {
        IEnumerable<CourseIndexViewModel> courses = await courseRepository
            .GetAllAttached()
            .AsNoTracking()
            .OrderBy(c=>c.CreatedAt)
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
            return ServiceResult.Fail("Невалидено ниво.", nameof(model.LevelId));
        }
         
        Level? level = await levelRepository
            .GetAllAttached()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == levelId);
         
        if (level == null)
        {
            return ServiceResult.Fail("Невалидено ниво.", nameof(model.LevelId));
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

    public Task<CourseDetailsViewModel> GetCourseDetailsByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}