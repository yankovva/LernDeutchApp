using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.UserLessonProgress;

namespace LerningApp.Services.Data.Interfaces;

public interface IUserLessonProgressService
{
    Task<ServiceResultT<IndexUserLessonProgressViewModel>> GetUserLessonProgress(Guid lessonId, string? userId);
    Task<ServiceResultT<int>> GetCourseProgressPercent(Guid courseId, Guid userId);
}