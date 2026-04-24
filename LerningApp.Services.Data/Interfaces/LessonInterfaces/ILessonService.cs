using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.Teacher;
using LerningApp.Web.ViewModels.Teacher.Lesson;

namespace LerningApp.Services.Data.Interfaces;

public interface ILessonService
{ 
    Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync();
    Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id, string userId);
    Task<ServiceResultT<AddLessonToCourseViewModel>> GetAddLessonToCourseByIdAsync(string id,string userId);
    Task<ServiceResult> AddLessonToCourseAsync(AddLessonToCourseViewModel model,string userId);
    Task<ServiceResult> AddLessonAsync(AddLessonInputModel model, string userId);
    Task<ServiceResultT<LessonEditInputModel>> GetLessonEditInputModelAsync(string id, string userId);
    Task<ServiceResult> PostLessonEditInputModelAsync(LessonEditInputModel model, string id, string userId);
    Task<ServiceResult> SoftDeleteLessonAsync(string id,string userId);
    Task<List<int>> GetAvailableOrderIndexes(string courseId);
    Task<IEnumerable<LessonTeacherIndexViewModel>> GetAllLessonsAsync();
    Task<ServiceResultT<LessonManageViewModel>> GetLessonManageByIdAsync(string id);
}