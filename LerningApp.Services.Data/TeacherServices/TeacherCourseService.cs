using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces.TeacherInterfaces;
using LerningApp.Web.ViewModels.Teacher;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data.TeacherServices;

public class TeacherCourseService(IRepository<Course, Guid> courserepository) : ITeacherCourseService
{
    public async Task<IEnumerable<CourseTeacherIndexViewModel>> GetAllCorsesAsync()
    {
        var courses = await courserepository
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
}