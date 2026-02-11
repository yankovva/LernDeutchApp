using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.LessonSection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class LessonController(LerningAppContext dbcontext) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        
        IEnumerable<LessonIndexViewModel> lessons =  await dbcontext.Lessons
            .AsNoTracking()
            .Include(l => l.Course) 
            .OrderBy(l => l.Name)
            .Select(l => new LessonIndexViewModel
            {
                Id = l.Id.ToString(),
                Name = l.Name,
                CourseId = l.CourseId.ToString(),
                CourseName = l.Course != null ? l.Course.Name : null,
                LevelName = l.Course.Level != null ? l.Course.Level.Name : null, 
                CreatedAt = l.CreatedAt.ToString("dd.MM.yyyy"),
            })
            .ToListAsync();
        
       return View(lessons);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        Guid lessonId = Guid.Empty;

        if (!IsGuidValid(id, ref lessonId))
        {
            return this.RedirectToAction(nameof(this.Index));
        }
        
        Lesson? lesson = await dbcontext
            .Lessons
            .AsNoTracking()
            .Include(l => l.Course)
            .Include(lesson => lesson.VocabularyCards)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        LessonContentViewModel model = new LessonContentViewModel()
        {
            Id = lesson.Id.ToString(),
            Name = lesson.Name,
            CourseId = lesson.CourseId.ToString(),
            Content = lesson.Content,
            WordCount = lesson.VocabularyCards.Count(),
            OrderIndex = lesson.OrderIndex,
            CourseName = lesson.Course != null ? lesson.Course.Name : "No course found.",
            Target = lesson.Target,
            LessonSections = await dbcontext.LessonsSections
                .Where(ls => ls.LessonId == lessonId)
                .OrderBy(ls => ls.OrderIndex)
                .Select(ls => new LessonSectionViewModel()
                {
                    Type = ls.Type,
                    OrderIndex = ls.OrderIndex,
                    Content = ls.Content,
                })
                .ToListAsync()
        };

        return this.View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AddToCourse(string id)
    {
        Guid lessonId = Guid.Empty;
        bool isIdValid = IsGuidValid(id, ref lessonId);

        if (!isIdValid)
        {
            return this.RedirectToAction(nameof(this.Index));
        }
        
        Lesson? lesson = await dbcontext
            .Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null) 
        {
            return this.RedirectToAction(nameof(this.Index));
        }

            AddLessonToCourseViewModel model = new AddLessonToCourseViewModel()
            {
                LessonId = lesson.Id.ToString(),
                LessonName = lesson.Name,
                SelectedCourseId = lesson.CourseId?.ToString().ToLower(),
                Courses = await dbcontext.Courses
                    .AsNoTracking()
                    .Select(c => new CourseCheckBoxItemInputModel
                    {
                        CourseId = c.Id.ToString().ToLower(),
                        CourseName = c.Name,
                    })
                    .ToListAsync()
            };
     
        return this.View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCourse(AddLessonToCourseViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }
        Guid lessonId = Guid.Empty;

        if (!this.IsGuidValid(model.LessonId, ref lessonId))
        {
            return this.RedirectToAction(nameof(this.Index));
        }
        
        Lesson? lesson = await dbcontext.
            Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return this.RedirectToAction(nameof(this.Index));
        }
        
        if (string.IsNullOrWhiteSpace(model.SelectedCourseId))
        {
            lesson.CourseId = null;
            await dbcontext.SaveChangesAsync();
            return RedirectToAction(nameof(this.Index));
        }
           
        Guid courseId = Guid.Empty;
        if (!this.IsGuidValid(model.SelectedCourseId, ref courseId))
        {
            ModelState.AddModelError(string.Empty, "Invalid course ID.");
            return View(model);
        }

        Course? course = await dbcontext
            .Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId);
        if (course == null)
        {
            ModelState.AddModelError(string.Empty, "Course not found.");
            return View(model);
        }

        lesson.CourseId = courseId;

        await dbcontext.SaveChangesAsync();
        return RedirectToAction(nameof(this.Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        AddLessonInputModel model = new AddLessonInputModel
        {
            Courses = await GetAllCoursesFromDbAsync()
        };
        return this.View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddLessonInputModel model)
    {
        if (!this.ModelState.IsValid)
        {
            model.Courses = await GetAllCoursesFromDbAsync();

            return View(model);
        }
        
        Guid courseId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(model.CourseId))
        {
            bool isCourseIdValid = IsGuidValid(model.CourseId, ref courseId);

            if (!isCourseIdValid)
            {
                ModelState.AddModelError(string.Empty, "Невалиден Course.");
                
                model.Courses = await GetAllCoursesFromDbAsync();

                return View(model);
            }
            
            Course? course = await dbcontext
                .Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);
            
            if (course != null)
            {
                courseId = Guid.Parse(model.CourseId);
            }
            else
            {
                ModelState
                    .AddModelError(nameof(model.CourseId), "Невалиден Course.");
                
                model.Courses = await GetAllCoursesFromDbAsync();;
                return this.View(model);
            }
        }
        
        Lesson lesson = new Lesson
        {
            Name = model.Name,
            Content = model.Content,
            CourseId = courseId,
            CreatedAt = DateTime.Now,
            OrderIndex = model.OrderIndex,
            Target = model.Target,
        };
        
        await  dbcontext.Lessons.AddAsync(lesson);
        await dbcontext.SaveChangesAsync();
        
        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        Guid lessonId = Guid.Empty;
        if (!IsGuidValid(id, ref lessonId))
        {
            return RedirectToAction(nameof(Index));
        }

        Lesson? lesson = await dbcontext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return RedirectToAction(nameof(Index));
        }
        
        var model = new LessonEditInputModel
        {
            Id = lesson.Id.ToString(),
            Name = lesson.Name,
            Content = lesson.Content,
            OrderIndex = lesson.OrderIndex,
            CourseId = lesson.CourseId?.ToString(),
            Target = lesson.Target,
            Courses = await GetAllCoursesFromDbAsync()
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(LessonEditInputModel model, string id)
{
    if (!ModelState.IsValid)
    {
        model.Courses = await GetAllCoursesFromDbAsync();
        return View(model);
    }

    Guid lessonId = Guid.Empty;
    if (!IsGuidValid(id, ref lessonId))
    {
        ModelState.AddModelError(string.Empty, "Невалиден урок.");
        model.Courses = await GetAllCoursesFromDbAsync();
        return View(model);
    }

    var lessonToChange = await dbcontext.Lessons
        .FirstOrDefaultAsync(l => l.Id == lessonId);

    if (lessonToChange == null)
    {
        ModelState.AddModelError(string.Empty, "Невалиден урок.");
        model.Courses = await GetAllCoursesFromDbAsync();
        return View(model);
    }

    Guid? courseId = null;
    if (!string.IsNullOrWhiteSpace(model.CourseId))
    {
        Guid parsedCourseId = Guid.Empty;
        if (!IsGuidValid(model.CourseId, ref parsedCourseId))
        {
            ModelState.AddModelError(nameof(LessonEditInputModel.CourseId), "Невалиден курс.");
            model.Courses = await GetAllCoursesFromDbAsync();
            return View(model);
        }

        bool courseExists = await dbcontext.Courses
            .AnyAsync(c => c.Id == parsedCourseId);
        if (!courseExists)
        {
            ModelState.AddModelError(nameof(LessonEditInputModel.CourseId), "Избраният курс не съществува.");
            model.Courses = await GetAllCoursesFromDbAsync();
            return View(model);
        }

        courseId = parsedCourseId;
    }

    lessonToChange.Name = model.Name;
    lessonToChange.Content = model.Content;
    lessonToChange.OrderIndex = model.OrderIndex;
    lessonToChange.CourseId = courseId;
    lessonToChange.Target = model.Target;
    
    await dbcontext.SaveChangesAsync();

    return RedirectToAction(nameof(Content), new { id = lessonId });
}

    private async Task<List<CourseOptionsViewModel>> GetAllCoursesFromDbAsync()
    {
        var courses = await dbcontext.Courses
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CourseOptionsViewModel
            {
                Id = c.Id.ToString(), 
                Name = c.Name
            })
            .ToListAsync();
        return courses;
    }
}