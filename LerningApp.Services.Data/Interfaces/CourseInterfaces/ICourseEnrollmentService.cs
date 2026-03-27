using LerningApp.Common;

namespace LerningApp.Services.Data.Interfaces;

public interface ICourseEnrollmentService
{
    Task<ServiceResult> EnrollInCourseAsync(string id, Guid userId);
}