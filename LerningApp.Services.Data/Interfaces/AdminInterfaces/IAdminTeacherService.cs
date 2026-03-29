using LerningApp.Common;
using LerningApp.Web.ViewModels.Admin.Teacher;

namespace LerningApp.Services.Data.Interfaces.AdminInterfaces;

public interface IAdminTeacherService
{
    Task<IEnumerable<AdminTeacherIndexViewModel>> GetAllTeachersAsync();
    Task<ServiceResultT<AdminTeacherDetailsViewModel>> GetTeacherDetailsAsync(string userId);
    Task<ServiceResult> AddPendingTeacherAsync(string userId);
    Task<ServiceResult> ApproveUserTeacherRoleAsync(string userId);
    Task<ServiceResult> RejectTeacherRequestAsync(string userId);
    Task<ServiceResult> RemoveTeacherRoleAsync(string userId);
    Task<ServiceResult> RemovePendingTeacherAsync(string teacherId);
    Task<ServiceResult> ReturnRemovedTeacher(string userId);
}