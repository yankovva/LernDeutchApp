using LerningApp.Common;
using LerningApp.Web.ViewModels.Teacher;

namespace LerningApp.Services.Data.Interfaces.TeacherInterfaces;

public interface ITeacherCourseService
{
    Task<IEnumerable<CourseTeacherIndexViewModel>> GetAllCorsesAsync();
    Task<ServiceResultT<CourseManageViewModel>> GetCourseManageByIdAsync(string id);
}