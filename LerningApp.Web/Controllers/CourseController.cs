using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Level;
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

    [HttpGet]
    public IActionResult Create()
    {
       
        var model = new AddCourseViewModel
        {
             Levels = this._dbcontext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new LevelOptionsViewModel
                {
                    Id = l.Id.ToString(),
                    Name = l.Name
                })
                .ToList()
        };

        return this.View(model);
    }
    
    [HttpPost]
    public IActionResult Create(AddCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = this._dbcontext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new LevelOptionsViewModel
                {
                    Id = l.Id.ToString(),
                    Name = l.Name
                })
                .ToList();

            return View(model);
        }
       

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            LevelId = model.LevelId!.Value,
            IsPublished = true,
            CreatedAt = DateTime.Now,
        };

        this._dbcontext.Courses.Add(course);
        this._dbcontext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}