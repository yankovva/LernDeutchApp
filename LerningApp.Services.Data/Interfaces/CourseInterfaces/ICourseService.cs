using LerningApp.Common;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Teacher;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseService
{ 
    Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(string? userId);
    Task<ServiceResult> AddCourseAsync(AddCourseViewModel model, string userId);
    Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId);
    Task<ServiceResultT<CourseEditViewModel>> GetCourseEditByIdAsync(string id, string userId);
    Task<ServiceResult> PostEditCourseAsync(CourseEditViewModel model, string id, string userId);
    Task<ServiceResult> DeactivateCourseAsync(string id, string userId);
    Task<ServiceResult> RestoreCourseAsync(string id, string userId);
    Task<ServiceResult> EnrollInCourseAsync(string id, Guid userId);
    Task<ServiceResult> SoftDeleteCourseAsync(string id, string userId);
    Task<ServiceResult> PublishCourseAsync(string id, string userId);
    Task<List<CourseOptionsViewModel>> GetAssignableCourseOptionsAsync();
    Task<ServiceResultT<CourseManageViewModel>> GetCourseManageByIdAsync(string id);
    Task<IEnumerable<CourseTeacherIndexViewModel>> GetAllCorsesAsync();
}