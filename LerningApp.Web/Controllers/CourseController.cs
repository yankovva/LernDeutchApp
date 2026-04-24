using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.Enums;

namespace LerningApp.Controllers;

public class CourseController(ICourseService courseService,
    ILevelService levelService,
    ITeacherService teacherService) : BaseController
{
    //Done
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId =  User.GetUserId();
       
        IEnumerable<CourseIndexViewModel> courses = await courseService
            .IndexGetCoursesAsync(userId);

        return this.View(courses);
    }

    //Done
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        string? userId = User.GetUserId();

        var result = await courseService.GetCourseDetailsByIdAsync(id, userId);

        if (result.Result == false)
        {
            TempData["ErrorMessage"] = $"{result.Message}";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }
   
    //Done
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(string courseId)
    {
        Guid userId = Guid.Parse(User.GetUserId()!);
        var result = await courseService.EnrollInCourseAsync(courseId, userId);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = $"Успешно се запизахте за курса.";
        return RedirectToAction("Details", new { id = courseId });
    }
    
   
}
