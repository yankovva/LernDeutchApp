using LerningApp.Web.ViewModels.Teacher;

namespace LerningApp.Services.Data.Interfaces.TeacherInterfaces;

public interface ITeacherCourseService
{
    Task<IEnumerable<CourseTeacherIndexViewModel>> GetAllCorsesAsync();
}