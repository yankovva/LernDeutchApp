using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using LerningApp.Web.ViewModels.Admin.Teacher;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data.AdminServices;

public class AdminTeacherService(UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IRepository<Teacher, Guid> teacherRepository) : IAdminTeacherService
{
    public async Task<IEnumerable<AdminTeacherIndexViewModel>> GetAllTeachersAsync()
    {
        var teachers = await teacherRepository
            .GetAllAttached()
            .Select(t => new AdminTeacherIndexViewModel()
            {
                Id = t.Id.ToString(),
                Email = t.User.Email,
                FirstName = t.User.FirstName,
                LastName = t.User.LastName,
                Status = t.Status.ToString(),
                TeacherSince = t.TeacherSince.HasValue
                    ? t.TeacherSince.Value.ToString("MM/dd/yyyy")
                    : "Pending"
            })
            .ToListAsync();

        return teachers;
    }
}