using LerningApp.Web.ViewModels.Course;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseService
{ 
    Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(Guid? userId);
    
    Task AddCourseAsync(AddCourseViewModel model);
    
    Task<CourseDetailsViewModel> GetCourseDetailsByIdAsync(Guid id);
}