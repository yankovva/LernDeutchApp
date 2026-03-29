using LerningApp.Services.Data.Interfaces.AdminInterfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Areas.Admin.Controllers;

[Area(AdminRole)]
[Authorize(Roles = AdminRole)]
public class TeacherController(IAdminTeacherService teacherService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await teacherService.GetAllTeachersNotDeletedAsync();
        return View(model);
    }
  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeTeacherRequest(string id)
    {
        var result = await teacherService.AddPendingTeacherAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        TempData["SuccessMessage"] = "Successfully created teacher request.";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string id)
    {
        var result = await teacherService.ApproveUserTeacherRoleAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        TempData["SuccessMessage"] = "Successfully added teacher role.";
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string id)
    {
        var result = await teacherService.RejectTeacherRequestAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        TempData["SuccessMessage"] = "Successfully rejected teacher request.";
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
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTeacherRequest(string id)
    {
        var result = await teacherService.RemovePendingTeacherAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        TempData["SuccessMessage"] = "Successfully removed teacher request.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnTeacher(string id)
    {
        var result = await teacherService.ReturnRemovedTeacher(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        TempData["SuccessMessage"] = "Successfully returned teacher.";
        return RedirectToAction(nameof(Index));
    }
    
    
}