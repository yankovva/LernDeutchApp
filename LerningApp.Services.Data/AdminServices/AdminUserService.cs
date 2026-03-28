using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using LerningApp.Web.ViewModels.Admin.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data.AdminServices;

public class AdminUserService(UserManager<ApplicationUser> userManager) : IAdminUserService
{
    public async Task<IEnumerable<AdminUserIndexViewModel>> GetAllUsersAsync()
    {
        var allUsers = await userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var result = new List<AdminUserIndexViewModel>();
        foreach (var u in allUsers)
        {
            var userLogs = await userManager.GetLoginsAsync(u);
            bool isFbUser = userLogs
                .Any(l => l.LoginProvider == "Facebook");
            
            result.Add(new AdminUserIndexViewModel()
            {
                Id = u.Id.ToString(),
                Email = u.Email,
                LastName = u.LastName,
                FirstName = u.FirstName,
                RegisteredOn = DateTime.UtcNow.ToString("MM/dd/yyyy"),
                IsFacebookUser = isFbUser
            });
        }
        
        return result;
    }
}