using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Teacher;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.Enums;

namespace LerningApp.Controllers;

[Authorize]
public class ProfileController(IProfileService profileService) : BaseController
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
        var result = await profileService
            .GetTeacherProfileIndexViewModelAsync(userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> TeacherEdit()
    {
        Guid userId = Guid.Parse(User.GetUserId()!);
        var result = await profileService
            .GetTeacherProfileEditViewModelAsync(userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        return View(result.Data);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TeacherEdit(ProfileEditViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }
        
        Guid userId = Guid.Parse(User.GetUserId()!);
       var result = await profileService.PostTeacherProfileEditAsync(userId, model);
       if (result.Result == false)
       {
           if (result.ErrorType == ServiceErrorType.Validation)
               ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
           else
               TempData["ErrorMessage"] = result.Message;
            
           return this.View(model);
       }
     
        TempData["SuccessMessage"] = "Your profile changes have been submitted for review.";
        return RedirectToAction(nameof(TeacherIndex));
    }
}
