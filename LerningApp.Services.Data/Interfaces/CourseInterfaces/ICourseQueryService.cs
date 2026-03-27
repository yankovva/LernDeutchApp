using LerningApp.Common;
using LerningApp.Web.ViewModels.Course;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseQueryService
{
    Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(string? userId);
    
    Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId);
    
    Task<List<CourseOptionsViewModel>> GetCourseOptionsAsync();
}