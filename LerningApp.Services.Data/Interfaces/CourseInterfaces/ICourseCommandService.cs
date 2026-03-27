using LerningApp.Common;
using LerningApp.Web.ViewModels.Course;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseCommandService
{
    Task<ServiceResult> AddCourseAsync(AddCourseViewModel model, string userId);
    
    Task<ServiceResultT<CourseEditViewModel>> GetCourseEditByIdAsync(string id, string userId);
    
    Task<ServiceResult> PostEditCourseAsync(CourseEditViewModel model, string id, string userId);
}