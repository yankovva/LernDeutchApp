using LerningApp.Data;
using LerningApp.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        
        return this.View(courses);
    }

    [HttpGet]
    public IActionResult Details(string id)
    {
        bool isIdValid = Guid.TryParse(id, out Guid Guidid);
        if (!isIdValid)
        {
            return this.RedirectToAction(nameof(this.Index));
        }
        
        Course? course = this._dbcontext
            .Courses
            .FirstOrDefault(c=>c.Id == Guidid);

        if (course == null)
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        return this.View(course);
    }
}