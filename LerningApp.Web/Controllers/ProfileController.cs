using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Teacher;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

[Authorize]
public class ProfileController(IProfileService profileService,
    IRepository<Teacher, Guid> teacherRepository,
    UserManager<ApplicationUser> userManager) : BaseController
{
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(this.User.GetUserId()!);
        var model = await profileService
            .IndexGetUserProfileOverviewModelAsync(userId);
        
        return View(model);
    }
     [HttpGet]
    public async Task<IActionResult> TeacherIndex()
    {
        Guid userId = Guid.Parse(User.GetUserId()!);
        var teacher = await teacherRepository
            .GetAllAttached()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId);
        
        if (teacher == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
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
            TeacherSince = teacher.TeacherSince.HasValue
                ? teacher.TeacherSince.Value.ToString("MM/dd/yyyy")
                : string.Empty,    
            Biography = teacher.Biography,
            Qualifications = teacher.Qualification
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> TeacherEdit()
    {
        Guid userId = Guid.Parse(User.GetUserId()!);

        var teacher = await teacherRepository
            .GetAllAttached()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return RedirectToAction("Index");
        }
        
        var model = new ProfileEditViewModel
        {
            TeacherId = teacher.Id.ToString(),
            UserId = teacher.UserId.ToString(),
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            PhoneNumber = teacher.User.PhoneNumber,
            ProfileImage = teacher.User.ProfileImage,
            Biography = teacher.Biography,
            Qualifications = teacher.Qualification
        };
            
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TeacherEdit(ProfileEditViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }
        
        if (!Guid.TryParse(model.TeacherId, out Guid teacherId))
        {
            return RedirectToAction(nameof(Index));
        }

        Guid userId = Guid.Parse(User.GetUserId()!);
        var user = await userManager.Users
            .Include(u => u.Teacher)
            .SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Teacher == null || user.Teacher.Id != teacherId)
        {
            return RedirectToAction(nameof(Index));
        }

        var teacher = await teacherRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return RedirectToAction(nameof(Index));
        }
        
        teacher.PendingFirstName = model.FirstName;
        teacher.PendingLastName = model.LastName;
        teacher.PendingPhoneNumber = model.PhoneNumber;
        teacher.PendingProfileImage = model.ProfileImage;
        teacher.PendingBiography = model.Biography;
        teacher.PendingQualification = model.Qualifications;
        
        if (teacher.Status == Enums.TeacherStatus.Approved)
        {
            teacher.HasProfileChangesPendingReview = true;
        }
        else if (teacher.Status == Enums.TeacherStatus.Draft || teacher.Status == Enums.TeacherStatus.Rejected)
        {
            teacher.HasProfileChangesPendingReview = false;
            teacher.Status = Enums.TeacherStatus.PendingReview;
        }
        
        await teacherRepository.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Your profile changes have been submitted for review.";
        return RedirectToAction(nameof(TeacherIndex));
    }
}