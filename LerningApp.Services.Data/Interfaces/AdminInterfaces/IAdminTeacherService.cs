using LerningApp.Common;
using LerningApp.Web.ViewModels.Admin.Teacher;

namespace LerningApp.Services.Data.Interfaces.AdminInterfaces;

public interface IAdminTeacherService
{
    Task<IEnumerable<AdminTeacherIndexViewModel>> GetAllTeachersNotDeletedAsync();
    Task<ServiceResultT<AdminTeacherDetailsViewModel>> GetTeacherDetailsAsync(string userId);
    Task<ServiceResult> AddPendingTeacherAsync(string userId);
    Task<ServiceResult> ApproveUserProfileChangesAsync(string userId);
    Task<ServiceResult> RejectTeacherChangesRequestAsync(string userId);
    Task<ServiceResult> RemoveTeacherRoleAsync(string userId);
    Task<ServiceResult> RemoveTeacherRequestAsync(string teacherId);
    Task<ServiceResult> ReturnRemovedTeacher(string userId);
}