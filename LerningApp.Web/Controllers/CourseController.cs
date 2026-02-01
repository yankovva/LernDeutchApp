using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Level;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class CourseController(LerningAppContext dbcontext) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
         IEnumerable<CourseIndexViewModel> courses = await dbcontext
            .Courses
            .AsNoTracking()
            .Select(c => new CourseIndexViewModel
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                LessonsCount = c.LessonsForCourse.Count,
                CourseLevel = c.Level.Name,
            })
            .ToListAsync();
        
         return this.View(courses);
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
        
        Course? course = await dbcontext
            .Courses
            .AsNoTracking()
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
       
        AddCourseViewModel model = new AddCourseViewModel
        {
             Levels =  await GetAllLevelsFromDbAsync()
        };

        return this.View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(AddCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = await GetAllLevelsFromDbAsync();

            return this.View(model);
        }
        
        Guid levelId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(model.LevelId))
        {
            bool isLevelIdValid = IsGuidValid(model.LevelId, ref levelId);
            if (!isLevelIdValid)
            {
                ModelState.AddModelError(string.Empty, "Невалидено ниво.");

                model.Levels = await GetAllLevelsFromDbAsync();

                return this.View(model);
            }
            
            Level? level = await dbcontext
                .Levels
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == levelId);
            
            if (level != null)
            {
                levelId = Guid.Parse(model.LevelId);
            }
            else
            {
                ModelState
                    .AddModelError(nameof(model.LevelId), "Невалидно Ниво.");

                model.Levels = await GetAllLevelsFromDbAsync();

                return this.View(model);
            }
        }

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            LevelId = levelId,
            IsPublished = true,
            CreatedAt = DateTime.Now,
        };

       await dbcontext.Courses.AddAsync(course);
       await  dbcontext.SaveChangesAsync();

       return RedirectToAction(nameof(Index));
    }
    private async Task<List<LevelOptionsViewModel>> GetAllLevelsFromDbAsync()
    {
        var levels = await dbcontext.Levels
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new LevelOptionsViewModel
            {
                Id = c.Id.ToString(), 
                Name = c.Name
            })
            .ToListAsync();
        
        return levels;
    }
}