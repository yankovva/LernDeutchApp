using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using LerningApp.Web.ViewModels.Admin.Teacher;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.Enums;

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
                UserId = t.UserId.ToString(),
                TeacherId = t.Id.ToString(),
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

    public async Task<ServiceResult> AddPendingTeacherAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.");
        }
        bool isTeacher = await userManager.IsInRoleAsync(user, "Teacher");
        if (!isTeacher)
        {
            var newTeacher = new Teacher()
            {
                UserId = Guid.Parse(id),
                Status = TeacherStatus.Draft
            };
            
            teacherRepository.Add(newTeacher);
            await teacherRepository.SaveChangesAsync();
        }
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> ApproveUserTeacherRoleAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.");
        }
        
        bool isTeacher = await userManager.IsInRoleAsync(user, "Teacher");
        if (isTeacher)
        {
            return ServiceResult.Fail("User already teacher.");
        }
        
        Guid userGuid = Guid.Parse(userId);
        var teacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == userGuid);
        if (teacher == null)
        {
            return ServiceResult.Fail("No teacher found.");
        }
        
        teacher.Status = TeacherStatus.Approved;
        await teacherRepository.SaveChangesAsync();
        await userManager.AddToRoleAsync(user, "Teacher");
        
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RejectTeacherRequestAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.");
        }
        bool isTeacher = await userManager.IsInRoleAsync(user, "Teacher");
        if (isTeacher)
        {
            return ServiceResult.Fail("User is already in role Teacher.");
        }
        
        Guid userGuid = Guid.Parse(userId);
        var teacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == userGuid);
        if (teacher == null)
        {
            return ServiceResult.Fail("No teacher request found.");
        }
        teacher.Status = TeacherStatus.Draft;
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveTeacherRoleAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.");
        }
        
        bool isTeacher = await userManager.IsInRoleAsync(user, "Teacher");
        if (!isTeacher)
        {
            return ServiceResult.Fail("User not in role teacher.");
        }
        Guid parsedUserId = Guid.Parse(userId);
        
        var teacher = await teacherRepository
            .FirstorDefaultAsync(u => u.UserId == parsedUserId);
        
        if (teacher == null)
        {
            return ServiceResult.Fail("User not a teacher.");
        }
        
        teacher.Status = TeacherStatus.Inactive;
        await teacherRepository.SaveChangesAsync();
        await userManager.RemoveFromRoleAsync(user, "Teacher");
        
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemovePendingTeacherAsync(string teacherId)
    {
        Guid id = Guid.TryParse(teacherId, out Guid teacherGuid)
            ? teacherGuid
            : Guid.Empty;
        
        Teacher? teacher = await teacherRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(t => t.Id == teacherGuid);
        if (teacher == null)
        {
            return ServiceResult.Fail("User does not have a pending teacher request.");
        }
        
        teacherRepository.DeleteByEntity(teacher);
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }

    public async  Task<ServiceResultT<AdminTeacherDetailsViewModel>> GetTeacherDetailsAsync(string teacherId)
    {
        Guid id = Guid.TryParse(teacherId, out Guid teacherGuid)
            ? teacherGuid
            : Guid.Empty;
        
        Teacher? teacher = await teacherRepository
            .GetAllAttached()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == teacherGuid);
        
        if (teacher == null)
        {
            return ServiceResultT<AdminTeacherDetailsViewModel>.Fail("User is not Teacher");
        }

        var result = new AdminTeacherDetailsViewModel()
        {
            UserId = teacher.UserId.ToString(),
            TeacherId = teacher.Id.ToString(),
            Email = teacher.User.Email,
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            UserName = teacher.User.UserName,
            PhoneNumber = teacher.User.PhoneNumber,
            Qualifications = teacher.Qualification,
            Status = teacher.Status.ToString(),
            Biography = teacher.Biography,
            TeacherSince = teacher.TeacherSince.HasValue
                ? teacher.TeacherSince.Value.ToString("MM/dd/yyyy")
                : "Pending",
            ProfileImage = teacher.User.ProfileImage
        };
        
        return ServiceResultT<AdminTeacherDetailsViewModel>.Success(result);
    }

    public async Task<ServiceResult> ReturnRemovedTeacher(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("No user found.");
        }
        Guid id = Guid.TryParse(userId, out Guid userGuid)
            ? userGuid
            : Guid.Empty;
        Teacher? userTeacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == userGuid);
        if (userTeacher == null)
        {
            return ServiceResult.Fail("No teacher found.");
        }
        
        userTeacher.Status = TeacherStatus.Draft;
        await teacherRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }
}