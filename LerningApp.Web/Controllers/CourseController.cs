using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Level;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class CourseController : BaseController
{
    private readonly LerningAppContext _dbcontext;

    public CourseController(LerningAppContext dbcontext)
    {
        this._dbcontext = dbcontext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
         IEnumerable<CourseIndexViewModel> courses = await this._dbcontext
            .Courses
            .Select(c => new CourseIndexViewModel
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                LessonsCount = c.LessonsForCourse.Count,
                CourseLevel = c.Level.Name,
            })
            .ToListAsync();
        
         return   this.View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        Guid courseId = Guid.Empty;

        bool isIdValid = IsGuidValid(id, ref courseId);

        if (!isIdValid)
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        Course? course = await this._dbcontext
            .Courses
            .Include(course => course.Level)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        CourseDetailsViewModel detailsViewModel = new CourseDetailsViewModel()
        {
            Id = course.Id.ToString(),
            Name = course.Name,
            Description = course.Description,
            LevelName = course.Level.Name,
        };

    return this.View(detailsViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
       
        var model = new AddCourseViewModel
        {
             Levels = await this._dbcontext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new LevelOptionsViewModel
                {
                    Id = l.Id.ToString(),
                    Name = l.Name
                })
                .ToListAsync()
        };

        return this.View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(AddCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = await  this._dbcontext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new LevelOptionsViewModel
                {
                    Id = l.Id.ToString(),
                    Name = l.Name
                })
                .ToListAsync();

            return View(model);
        }
       

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            LevelId = Guid.Parse(model.LevelId!),
            IsPublished = true,
            CreatedAt = DateTime.Now,
        };

       await this._dbcontext.Courses.AddAsync(course);
       await  this._dbcontext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    
}