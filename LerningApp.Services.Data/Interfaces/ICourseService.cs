using LerningApp.Common;
using LerningApp.Web.ViewModels.Course;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseService
{ 
    Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(Guid? userId);
    
    Task<ServiceResult> AddCourseAsync(AddCourseViewModel model);
    
    Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId);

    Task<ServiceResultT<CourseEditViewModel>> GetCourseEditByIdAsync(string id);
    
    Task<ServiceResult> PostEditCourseAsync(CourseEditViewModel model, string id);
    
    Task<ServiceResult> DeactivateCourseAsync(string id);
    
    Task<ServiceResult> RestoreCourseAsync(string id);
    Task<ServiceResult> EnrollInCourseAsync(string id, Guid userId);
}