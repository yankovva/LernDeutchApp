using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

public class Course : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}