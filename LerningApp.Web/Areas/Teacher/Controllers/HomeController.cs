using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Areas.Teacher.Controllers;

[Area(TeacherRole)]
[Authorize(Roles = "Admin,Teacher")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}