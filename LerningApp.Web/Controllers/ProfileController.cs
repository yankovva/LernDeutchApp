using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}