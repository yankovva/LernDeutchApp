using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.Teacher;

namespace LerningApp.Services.Data.Interfaces.LessonInterfaces;

public interface ILessonQueryService
{
    Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync();
    
    Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id, string userId);
    
    Task<List<int>> GetAvailableOrderIndexes(string courseId);
    Task<IEnumerable<LessonTeacherIndexViewModel>> GetAllLessonsAsync();
}