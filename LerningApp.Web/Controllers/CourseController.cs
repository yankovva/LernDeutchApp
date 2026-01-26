using LerningApp.Data;
using LerningApp.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

public class CourseController : Controller
{
    private readonly LerningAppContext _dbcontext;

    public CourseController(LerningAppContext dbcontext)
    {
        this._dbcontext = dbcontext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        IEnumerable<Course> courses = this._dbcontext
            .Courses
            .ToList();
        
        return View(courses);
    }
}