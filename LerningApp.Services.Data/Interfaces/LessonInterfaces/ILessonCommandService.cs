using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;

namespace LerningApp.Services.Data.Interfaces.LessonInterfaces;

public interface ILessonCommandService
{
    Task<ServiceResult> AddLessonAsync(AddLessonInputModel model, string userId);
    
    Task<ServiceResultT<LessonEditInputModel>> GetLessonEditInputModelAsync(string id, string userId);
    
    Task<ServiceResult> PostLessonEditInputModelAsync(LessonEditInputModel model, string id, string userId);
}