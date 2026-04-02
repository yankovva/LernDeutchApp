using LerningApp.Services.Data.Interfaces.TeacherInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Areas.Teacher.Controllers;

[Area(TeacherRole)]
[Authorize(Roles = "Admin,Teacher")]
public class LessonController(ITeacherLessonService teacherLessonService) : Controller
{
   [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lessons = await teacherLessonService.GetAllLessonsAsync();
        return View(lessons);
    }
}