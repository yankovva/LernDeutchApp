using LerningApp.Common;
using LerningApp.Web.ViewModels.Lesson;

namespace LerningApp.Services.Data.Interfaces.LessonInterfaces;

public interface ILessonCourseAssignmentService
{
    Task<ServiceResultT<AddLessonToCourseViewModel>> GetAddLessonToCourseByIdAsync(string id,string userId);
    
    Task<ServiceResult> AddLessonToCourseAsync(AddLessonToCourseViewModel model,string userId);
}