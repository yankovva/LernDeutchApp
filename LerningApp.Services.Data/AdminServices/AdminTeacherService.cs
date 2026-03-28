using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using LerningApp.Web.ViewModels.Admin.Teacher;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data.AdminServices;

public class AdminTeacherService(UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IAdminTeacherService
{
    public async Task<IEnumerable<AdminTeacherIndexViewModel>> GetAllTeachersAsync()
    {
        var approvedTeachers = await userManager
            .GetUsersInRoleAsync("Teacher");
        
        var teachers = approvedTeachers
            .Select(t => new AdminTeacherIndexViewModel
        {
            Id = t.Id.ToString(),
            Email = t.Email,
            FirstName = t.FirstName,
            LastName = t.LastName,
            IsApproved = true,
            TeacherSince = DateTime.UtcNow.ToString("MM/dd/yyyy"),
        }).ToList();

        var notApprovedTeachers = await userManager.Users
            .Where(u => u.Teacher != null && u.Teacher.IsApproved == false)
            .Select(t => new AdminTeacherIndexViewModel
            {
                Id = t.Id.ToString(),
                Email = t.Email,
                FirstName = t.FirstName,
                LastName = t.LastName,
                IsApproved = false,
                TeacherSince = DateTime.UtcNow.ToString("MM/dd/yyyy"),
            }).ToListAsync();

        teachers.AddRange(notApprovedTeachers);
        return teachers;
    }
}