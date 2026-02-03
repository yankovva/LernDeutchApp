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
                IsActive = c.IsPublished
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
            IsActive = course.IsPublished,
        };

        return this.View(detailsViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {

        AddCourseViewModel model = new AddCourseViewModel
        {
            Levels = await GetAllLevelsFromDbAsync()
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
        await dbcontext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
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
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        CourseEditViewModel model = new CourseEditViewModel()
        {
            Id = courseId.ToString(),
            Name = course.Name,
            Description = course.Description,
            Levels = await GetAllLevelsFromDbAsync(),
            LevelId = course.LevelId.ToString()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CourseEditViewModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            model.Levels = await GetAllLevelsFromDbAsync();
            return this.View(model);
        }

        Guid courseId = Guid.Empty;
        if (!IsGuidValid(id, ref courseId))
        {
            ModelState.AddModelError(string.Empty, "Невалиден Курс.");
            model.Levels = await GetAllLevelsFromDbAsync();
            return this.View(model);
        }

        Course? courseToChange = dbcontext
            .Courses
            .FirstOrDefault(c => c.Id == courseId);

        if (courseToChange == null)
        {
            ModelState.AddModelError(string.Empty, "Невалиден Курс.");
            model.Levels = await GetAllLevelsFromDbAsync();
            return this.View(model);
        }

        Guid levelId = Guid.Empty;

        if (!IsGuidValid(model.LevelId, ref levelId))
        {
            ModelState.AddModelError(nameof(model.LevelId), "Невалидно ниво.");
            model.Levels = await GetAllLevelsFromDbAsync();
            return View(model);
        }

        bool levelExists = await dbcontext.Levels
            .AnyAsync(l => l.Id == levelId);

        if (!levelExists)
        {
            ModelState.AddModelError(nameof(model.LevelId), "Избраното ниво не съществува.");
            model.Levels = await GetAllLevelsFromDbAsync();
            return View(model);
        }

        courseToChange.Name = model.Name;
        courseToChange.Description = model.Description;
        courseToChange.LevelId = levelId;

        await dbcontext.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { Id = courseId });
    }

    public async Task<IActionResult> Deactivate(string id)
    {
        Guid courseId = Guid.Empty;
        if (!IsGuidValid(id, ref courseId))
        {
            ModelState.AddModelError(string.Empty, "Невалиден Курс.");
            return RedirectToAction(nameof(Index));
        }
        var course = await dbcontext
            .Courses
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return RedirectToAction(nameof(Index));
        }

        course.IsPublished = false;
        await dbcontext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Restore(string id)
    {
        Guid courseId = Guid.Empty;
        if (!IsGuidValid(id, ref courseId))
        {
            ModelState.AddModelError(string.Empty, "Невалиден Курс.");
            return RedirectToAction(nameof(Index));
        }
        var course = await dbcontext
            .Courses
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return RedirectToAction(nameof(Index));
        }

        course.IsPublished = true;
        await dbcontext.SaveChangesAsync();

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