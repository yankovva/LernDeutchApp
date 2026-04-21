using LerningApp.Common;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseLifecycleService
{
    Task<ServiceResult> DeactivateCourseAsync(string id, string userId);
    Task<ServiceResult> RestoreCourseAsync(string id, string userId);
    Task<ServiceResult> SoftDeleteCourseAsync(string id, string userId);
    Task<ServiceResult> PublishCourseAsync(string id, string userId);

}