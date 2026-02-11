using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Level;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class CourseController(LerningAppContext dbcontext, UserManager<ApplicationUser> userManager) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(this.User);
        Guid? userGuidId = null;
        if (!string.IsNullOrWhiteSpace(userId))
        {
            userGuidId = Guid.Parse(userId);
        }

        IEnumerable<CourseIndexViewModel> courses = await dbcontext
            .Courses
            .AsNoTracking()
            .Select(c => new CourseIndexViewModel
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                LessonsCount = c.LessonsForCourse.Count,
                CourseLevel = c.Level.Name,
                IsActive = c.IsPublished,
                IsEnrolled = userId != null && dbcontext
                    .UsersCourses
                    .Any(c => c.UserId == userGuidId && c.CourseId == c.CourseId)
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
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return this.RedirectToAction(nameof(this.Index));
        }
        
        Course? course = await dbcontext
            .Courses
            .AsNoTracking()
            .Include(course => course.Level)
            .Include(course => course.LessonsForCourse)
            .ThenInclude(lesson => lesson.VocabularyCards).Include(course => course.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return this.RedirectToAction(nameof(this.Index));
        }

        CourseDetailsViewModel model =  new CourseDetailsViewModel()
        {
            Id = course.Id.ToString(),
            Name = course.Name,
            Description = course.Description,
            LevelName = course.Level.Name,
            IsActive = course.IsPublished,
            CourseLessons = course.LessonsForCourse
                .Select(cl=>  new CourseLessonsViewModel()
                {
                    LessinId = cl.Id.ToString(),
                    LessonName = cl.Name,
                    WordsInLesson = cl.VocabularyCards.Count() ,
                    LessonTarget = cl.Target
                }).ToList()
        };
        
        string? userId = userManager.GetUserId(User);

        if (userId != null)
        {
            model.IsEnrolled = await dbcontext
                .UsersCourses
                .AnyAsync(uc => uc.UserId == Guid.Parse(userId) && uc.CourseId == courseId);
        }
        
        return this.View(model);
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
                ModelState.AddModelError(nameof(model.LevelId), "Невалидно Ниво.");

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
        
        TempData["SuccessMessage"] = "Успешно създадохте курс.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        Guid courseId = Guid.Empty;
        bool isIdValid = IsGuidValid(id, ref courseId);
        if (!isIdValid)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return this.RedirectToAction(nameof(this.Index));
        }

        Course? course = await dbcontext
            .Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
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
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            model.Levels = await GetAllLevelsFromDbAsync();
            return this.View(model);
        }

        Course? courseToChange = dbcontext
            .Courses
            .FirstOrDefault(c => c.Id == courseId);

        if (courseToChange == null)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";          
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

        TempData["SuccessMessage"] = "Успешно редактирахте курса.";
        return RedirectToAction(nameof(Details), new { Id = courseId });
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(string id)
    {
        Guid courseId = Guid.Empty;
        if (!IsGuidValid(id, ref courseId))
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }
        var course = await dbcontext
            .Courses
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }

        course.IsPublished = false;
        await dbcontext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> Restore(string id)
    {
        Guid courseId = Guid.Empty;
        if (!IsGuidValid(id, ref courseId))
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }
        var course = await dbcontext
            .Courses
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }

        course.IsPublished = true;
        await dbcontext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Успешно деактивирахте курса.";

        return RedirectToAction(nameof(Index));
    }
  
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Enroll(string courseId)
    { 
        var userId = Guid.Parse(userManager.GetUserId(User)!);
        
        Guid courseGuidId = Guid.Empty;
        if (!IsGuidValid(courseId, ref courseGuidId))
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }

        Course? course = await dbcontext
            .Courses
            .FirstOrDefaultAsync(c => c.Id == courseGuidId && c.IsPublished == true);

        if (course == null)
        {
            TempData["ErrorMessage"] = "Курсът не е намерен.";
            return RedirectToAction(nameof(Index));
        }
        
        bool alreadyEnrolled = await dbcontext.UsersCourses
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseGuidId);

        if (alreadyEnrolled)
        {
            TempData["InfoMessage"] = "Вече сте записани за този курс.";
            return RedirectToAction("Details", new { id = courseId });
        }

        UserCourse newUserCourse = new UserCourse
        {
            UserId = userId,
            CourseId = courseGuidId,
            StartedAt = DateTime.UtcNow
        };

        await dbcontext.UsersCourses.AddAsync(newUserCourse);
        await dbcontext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = $"Успешно се запизахте за курс {course.Name}.";
        return RedirectToAction("Details", new { id = courseId });
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