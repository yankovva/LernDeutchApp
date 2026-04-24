using LerningApp.Common;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.LessonInterfaces;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.Teacher;
using LerningApp.Web.ViewModels.Teacher.Lesson;

namespace LerningApp.Services.Data;

public class LessonService(ILessonCommandService commandService,
    ILessonCourseAssignmentService courseAssignmentService,
    ILessonLifecycleService lifecycleService,
    ILessonQueryService queryService) : ILessonService
{
    public async Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync()
    {
       return  await  queryService.IndexGetLessonsAsync();
    }

    public async Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id, string userId)
    {
       return  await queryService.GetLessonDetailsAsync(id, userId);
    }

    public async Task<ServiceResultT<AddLessonToCourseViewModel>> GetAddLessonToCourseByIdAsync(string id,string userId)
    {
       return await courseAssignmentService.GetAddLessonToCourseByIdAsync(id, userId);    
    }

    public async Task<ServiceResult> AddLessonToCourseAsync(AddLessonToCourseViewModel model, string userId)
    {
       return await courseAssignmentService.AddLessonToCourseAsync(model, userId);
    }

    //TODO add logic for the order of the lessons in a course
    public async Task<ServiceResult> AddLessonAsync(AddLessonInputModel model, string userId)
    { 
       return await commandService.AddLessonAsync(model, userId);
    }

    public async Task<ServiceResultT<LessonEditInputModel>> GetLessonEditInputModelAsync(string id, string userId)
    {
        return await commandService.GetLessonEditInputModelAsync(id, userId);
    }

    public async Task<ServiceResult> PostLessonEditInputModelAsync(LessonEditInputModel model, string id, string userId)
    {
       return await commandService.PostLessonEditInputModelAsync(model, id, userId);
    }

    public async Task<ServiceResult> SoftDeleteLessonAsync(string id, string userId)
    {
       return await lifecycleService.SoftDeleteLessonAsync(id, userId);
    }

    public async Task<List<int>> GetAvailableOrderIndexes(string courseId)
    {
       return await queryService.GetAvailableOrderIndexes(courseId);
    }

    public async Task<IEnumerable<LessonTeacherIndexViewModel>> GetAllLessonsAsync()
    {
       return await queryService.GetAllLessonsAsync();
    }

    public async Task<ServiceResultT<LessonManageViewModel>> GetLessonManageByIdAsync(string id)
    {
       return await queryService.GetLessonManageByIdAsync(id);
    }
}
