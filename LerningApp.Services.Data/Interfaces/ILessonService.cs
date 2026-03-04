using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;

namespace LerningApp.Services.Data.Interfaces;

public interface ILessonService
{ 
    Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync();
    Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id);
    Task<ServiceResultT<AddLessonToCourseViewModel>> GetAddLessonToCourseByIdAsync(string id);
    Task<ServiceResult> AddLessonToCourseAsync(AddLessonToCourseViewModel model);
    Task<ServiceResult> AddLessonAsync(AddLessonInputModel model, string userId);
    Task<ServiceResultT<LessonEditInputModel>> GetLessonEditInputModelAsync(string id);
    Task<ServiceResult> PostLessonEditInputModelAsync(LessonEditInputModel model, string id);
    Task<ServiceResult> SoftDeleteLessonAsync(string id);
}