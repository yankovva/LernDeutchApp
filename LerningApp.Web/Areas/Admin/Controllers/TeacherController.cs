using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Areas.Admin.Controllers;

public class TeacherController(IAdminTeacherService teacherService) : Controller
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var model = await teacherService.GetAllTeachersAsync();
        return View(model);
    }
}