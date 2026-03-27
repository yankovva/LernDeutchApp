using LerningApp.Common;

namespace LerningApp.Services.Data.Interfaces.LessonInterfaces;

public interface ILessonLifecycleService
{
    Task<ServiceResult> SoftDeleteLessonAsync(string id,string userId);
}