using LerningApp.Common;
using LerningApp.Web.ViewModels.Admin.Teacher;

namespace LerningApp.Services.Data.Interfaces.AdminInterfaces;

public interface IAdminTeacherService
{
    Task<IEnumerable<AdminTeacherIndexViewModel>> GetAllTeachersAsync();
    Task<ServiceResult> AddPendingTeacherAsync(string userId);
    Task<ServiceResult> AddUserTeacherRoleAsync(string userId);
    Task<ServiceResult> RemoveTeacherRoleAsync(string userId);
    Task<ServiceResult> RemovePendingTeacherAsync(string teacherId);
    Task<ServiceResultT<AdminTeacherDetailsViewModel>> GetTeacherDetailsAsync(string userId);
    Task<ServiceResult> ReturnRemovedTeacher(string userId);
}