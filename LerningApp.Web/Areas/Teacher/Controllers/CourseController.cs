using LerningApp.Services.Data.Interfaces.TeacherInterfaces;
using LerningApp.Services.Data.TeacherServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Areas.Teacher.Controllers;

[Area(TeacherRole)]
[Authorize(Roles = "Admin,Teacher")]
public class CourseController(ITeacherCourseService teacherCourseService) : Controller
{
   
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var courses = await teacherCourseService.GetAllCorsesAsync();
        return View(courses);
    }
}