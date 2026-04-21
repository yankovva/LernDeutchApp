using LerningApp.Common;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;

namespace LerningApp.Services.Data;

public class CourseService(
   ICourseCommandService commandService,
   ICourseEnrollmentService enrollmentService,
   ICourseLifecycleService lifecycleService,
   ICourseQueryService queryService) : ICourseService
{
    public async Task<IEnumerable<CourseIndexViewModel>> IndexGetCoursesAsync(string? userId)
    {
       return await queryService.IndexGetCoursesAsync(userId);
    }

    public async Task<ServiceResult> AddCourseAsync(AddCourseViewModel model, string userId)
    {
       return await commandService.AddCourseAsync(model, userId);
    }

    public async Task<ServiceResultT<CourseDetailsViewModel>> GetCourseDetailsByIdAsync(string id, string? userId)
    {
        return await queryService.GetCourseDetailsByIdAsync(id, userId);
    }

    //TODO Course Edit -> Reorder Lessons (drag&drop).
    public async Task<ServiceResultT<CourseEditViewModel>> GetCourseEditByIdAsync(string id, string userId)
    {
        return await commandService.GetCourseEditByIdAsync(id, userId);
    }

    public async Task<ServiceResult> PostEditCourseAsync(CourseEditViewModel model, string id, string userId)
    {
        return await commandService.PostEditCourseAsync(model, id, userId);
    }

    public async Task<ServiceResult> DeactivateCourseAsync(string id,string userId)
    {
       return await lifecycleService.DeactivateCourseAsync(id, userId);
    }

    public async Task<ServiceResult> RestoreCourseAsync(string id, string userId)
    {
       return await lifecycleService.RestoreCourseAsync(id, userId);
    }

    public async Task<ServiceResult> EnrollInCourseAsync(string id, Guid userId)
    {
       return await enrollmentService.EnrollInCourseAsync(id, userId);
    }

    public async Task<ServiceResult> SoftDeleteCourseAsync(string id, string userId)
    {
       return await lifecycleService.SoftDeleteCourseAsync(id, userId);
    }
    public async Task<List<CourseOptionsViewModel>> GetAssignableCourseOptionsAsync()
    {
        return await queryService.GetAssignableCourseOptionsAsync();
    }
    public async Task<ServiceResult> PublishCourseAsync(string id, string userId)
    {
        return await lifecycleService.PublishCourseAsync(id, userId);
    }
}