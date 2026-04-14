using LerningApp.Data.Repository.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Teacher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Areas.Teacher.Controllers;

[Area(TeacherRole)]
[Authorize(Roles = "Teacher")]
public class ProfileController(IRepository<Data.Models.Teacher, Guid> teacherRepository) : Controller
{
    public async Task<IActionResult> Index()
    {
        Guid userId = Guid.Parse(User.GetUserId()!);
        var teacher = await teacherRepository
            .GetAllAttached()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId);
        
        var model = new ProfileIndexViewModel
        {
            TeacherId = teacher.Id.ToString(),
            UserId = teacher.UserId.ToString(),
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            UserName = teacher.User.UserName,
            Email = teacher.User.Email,
            PhoneNumber = teacher.User.PhoneNumber,
            ProfileImage = teacher.User.ProfileImage,
            Status = teacher.Status.ToString(),
            TeacherSince = teacher.TeacherSince != null ? teacher.TeacherSince.ToString() : string.Empty,
            Biography = teacher.Biography,
            Qualifications = teacher.Qualification
        };
        return View(model);
    }
}