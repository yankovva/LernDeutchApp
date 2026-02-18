using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;

namespace LerningApp.Services.Data.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync();

    Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id);
}