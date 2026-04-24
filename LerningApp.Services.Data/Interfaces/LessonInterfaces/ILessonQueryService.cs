using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.Teacher;
using LerningApp.Web.ViewModels.Teacher.Lesson;

namespace LerningApp.Services.Data.Interfaces.LessonInterfaces;

public interface ILessonQueryService
{
    Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync();
    Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id, string userId);
    Task<List<int>> GetAvailableOrderIndexes(string courseId);
    Task<IEnumerable<LessonTeacherIndexViewModel>> GetAllLessonsAsync();
    Task<ServiceResultT<LessonManageViewModel>> GetLessonManageByIdAsync(string id);
}