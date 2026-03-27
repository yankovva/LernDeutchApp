using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;

namespace LerningApp.Services.Data.Interfaces.LessonInterfaces;

public interface ILessonQueryService
{
    Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync();
    
    Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id, string userId);
    
    Task<List<int>> GetAvailableOrderIndexes(string courseId);
}