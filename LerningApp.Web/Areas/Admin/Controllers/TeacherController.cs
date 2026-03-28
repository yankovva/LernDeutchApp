using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TeacherController(IAdminTeacherService teacherService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await teacherService.GetAllTeachersAsync();
        return View(model);
    }
  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeTeacher(string id)
    {
        var result = await teacherService.MakeUserTeacherAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        TempData["SuccessMessage"] = "Successfully made teacher";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var result = await teacherService.GetTeacherDetailsAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTeacherRole(string id)
    {
        var result = await teacherService.RemoveTeacherRoleAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        TempData["SuccessMessage"] = "Successfully removed user form teacher role";
        return RedirectToAction(nameof(Index));
    }
    
}