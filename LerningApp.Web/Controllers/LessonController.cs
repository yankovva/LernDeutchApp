using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.Enums;
namespace LerningApp.Controllers;

[Authorize]
public class LessonController(ILessonService lessonService,
    ICourseService courseService,
    ITeacherService teacherService) : BaseController
{

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        string? userId = User.GetUserId()!;
        var result = await  lessonService.GetLessonDetailsAsync(id, userId);
        if (result.Result == false )
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return this.View(result.Data);
    }
}
