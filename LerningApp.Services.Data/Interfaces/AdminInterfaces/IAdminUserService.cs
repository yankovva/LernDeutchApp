using LerningApp.Common;
using LerningApp.Web.ViewModels.Admin.User;

namespace LerningApp.Services.Data.Interfaces.AdminInterfaces;

public interface IAdminUserService
{
    Task<IEnumerable<AdminUserIndexViewModel>> GetAllUsersNotDeletedAsync();
    Task<ServiceResult>DeleteUserAsync(string userId);
}