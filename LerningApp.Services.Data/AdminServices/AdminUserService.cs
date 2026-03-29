using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using LerningApp.Web.ViewModels.Admin.User;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data.AdminServices;

public class AdminUserService(UserManager<ApplicationUser> userManager,
    IFileService fileService,
    IRepository<Teacher,Guid> teacherRepository) : IAdminUserService
{
    public async Task<IEnumerable<AdminUserIndexViewModel>> GetAllUsersNotDeletedAsync()
    {
        var allUsers = await userManager.Users
            .Where(u => u.IsDeleted == false)
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var result = new List<AdminUserIndexViewModel>();
        foreach (var u in allUsers)
        {
            var userLogs = await userManager.GetLoginsAsync(u);
            bool isFbUser = userLogs
                .Any(l => l.LoginProvider == "Facebook");
            var userRoles = await userManager.GetRolesAsync(u);
            result.Add(new AdminUserIndexViewModel()
            {
                Id = u.Id.ToString(),
                Email = u.Email,
                LastName = u.LastName,
                Roles = userRoles,
                UserName = u.UserName,
                FirstName = u.FirstName,
                RegisteredOn = DateTime.UtcNow.ToString("MM/dd/yyyy"),
                IsFacebookUser = isFbUser
            });
        }
        
        return result;
    }

    public async Task<ServiceResult> DeleteUserAsync(string userId)
    {
        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return ServiceResult.Fail("Invalid user id.");
        }

        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
           return ServiceResult.Fail("User not found.");
        }
        
        string? userPhoto = user.ProfileImage;
        if (userPhoto != null)
        {
            fileService.DeleteFile(userPhoto);
        }
        
        var userTeacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == parsedUserId);
        
        if ((userTeacher != null) && userTeacher.Status == Enums.TeacherStatus.Approved)
        {
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                var removedRole = await userManager.RemoveFromRoleAsync(user, role);
                if (!removedRole.Succeeded)
                {
                    return ServiceResult.Fail($"Failed to remove user role {role}.");
                }
            }

            var logins = await userManager.GetLoginsAsync(user);
            foreach (var login in logins)
            {
               var removedLogins = await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
               if (!removedLogins.Succeeded)
               {
                   return ServiceResult.Fail("Failed to delete user logins.");
               }
            }
            
            user.LastName = "User";
            user.FirstName = "Deleted";
            user.ProfileImage = null;
            user.Email = $"deleted-{user.Id}@deleted.local";
            user.UserName = $"deleted-{user.Id}";
            user.NormalizedEmail = user.Email.ToUpper();
            user.NormalizedUserName = user.UserName.ToUpper();
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.PhoneNumber = null;
            user.PasswordHash = null;
            user.EmailConfirmed = false;
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            userTeacher.Status = Enums.TeacherStatus.Inactive;
            
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return ServiceResult.Fail("Failed to anonymize user.");
            }

            await teacherRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }
        
        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail("Failed to delete user.");
        }
        
        return ServiceResult.Success();
    }
}