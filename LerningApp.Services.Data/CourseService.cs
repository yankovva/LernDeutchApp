using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data;

public class CourseService(IRepository<Course, Guid> courseRepository) : ICourseService
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

    public Task AddCourseAsync(AddCourseViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<CourseDetailsViewModel> GetCourseDetailsByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}