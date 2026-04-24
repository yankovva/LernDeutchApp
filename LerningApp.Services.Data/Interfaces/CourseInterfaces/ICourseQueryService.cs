using LerningApp.Common;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Teacher;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseQueryService
{
    Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(string? userId);
    
    Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId);
    
    Task<List<CourseOptionsViewModel>> GetAssignableCourseOptionsAsync();

    Task<IEnumerable<CourseTeacherIndexViewModel>> GetAllCorsesAsync();

    Task<ServiceResultT<CourseManageViewModel>> GetCourseManageByIdAsync(string id);
}