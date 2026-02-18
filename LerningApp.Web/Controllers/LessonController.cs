using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.LessonSection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class LessonController(LerningAppContext dbcontext, 
    ILessonService lessonService,
    ICourseService courseService,
    UserManager<ApplicationUser> userManager) : BaseController
{
    
    [HttpGet]
    public async Task<IActionResult> Index()
    { 
        var lessons = await lessonService.IndexGetLessonsAsync();
        
        return View(lessons);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var result = await  lessonService.GetLessonDetailsAsync(id);
        if (result.Result == false )
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        
        return this.View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> AddToCourse(string id)
    {
        var result = await lessonService.GetAddLessonToCourseByIdAsync(id);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        
        return this.View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCourse(AddLessonToCourseViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }
        
        var result = await lessonService.AddLessonToCourseAsync(model);
        if (result.Result == false)
        {
            if (result.Field != null)
                ModelState.AddModelError(string.Empty, result.Message);
            else
                TempData["ErrorMessage"] = result.Message;
            
            return this.View(model);
        }
        // TODO: consider enum for result action
        if (string.IsNullOrWhiteSpace(model.SelectedCourseId))
            TempData["SuccessMessage"] = "Урокът беше премахнат от курса.";
        else
            TempData["SuccessMessage"] = "Урокът беше добавен към курса.";
        return RedirectToAction(nameof(this.Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        AddLessonInputModel model = new AddLessonInputModel
        {
            Courses = await courseService.GetCourseOptionsAsync()
        };
        return this.View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(AddLessonInputModel model)
    {
        Guid userId = Guid.Parse(userManager.GetUserId(User)!);
        
        if (!this.ModelState.IsValid)
        {
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }

        var result = await  lessonService.AddLessonAsync(model, userId);
        if (result.Result == false)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }
        
        TempData["SuccessMessage"] = $"Успешно създадохте {model.Name}.";
        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        Guid lessonId = Guid.Empty;
        if (!IsGuidValid(id, ref lessonId))
        {
            TempData["ErrorMessage"] = "Невалиден урок.";
            return RedirectToAction(nameof(Index));
        }

        Lesson? lesson = await dbcontext.Lessons
            .AsNoTracking()
            .Include(lesson => lesson.LessonSections)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            TempData["ErrorMessage"] = "Невалиден урок.";
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
            Grammar = lesson.LessonSections?
                .FirstOrDefault(ls => ls.Type == "grammar")?.Content ?? "Add new grammar",
            Exercise = lesson.LessonSections?
            .FirstOrDefault(ls => ls.Type == "exercise")?.Content ?? "Add new exercise" ,
            Courses = await courseService.GetCourseOptionsAsync()
        };
            
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(LessonEditInputModel model, string id)
{
    if (!ModelState.IsValid)
    {
        model.Courses = await courseService.GetCourseOptionsAsync();
        return View(model);
    }

    Guid lessonId = Guid.Empty;
    if (!IsGuidValid(id, ref lessonId))
    {
        ModelState.AddModelError(string.Empty, "Невалиден урок.");
        model.Courses = await courseService.GetCourseOptionsAsync();
        return View(model);
    }

    Lesson? lessonToChange = await dbcontext
        .Lessons
        .Include(lesson => lesson.LessonSections)
        .FirstOrDefaultAsync(l => l.Id == lessonId);

    if (lessonToChange == null)
    {
        ModelState.AddModelError(string.Empty, "Невалиден урок.");
        model.Courses = await courseService.GetCourseOptionsAsync();
        return View(model);
    }

    Guid? courseId = null;
    if (!string.IsNullOrWhiteSpace(model.CourseId))
    {
        Guid parsedCourseId = Guid.Empty;
        if (!IsGuidValid(model.CourseId, ref parsedCourseId))
        {
            ModelState.AddModelError(nameof(LessonEditInputModel.CourseId), "Невалиден курс.");
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }

        bool courseExists = await dbcontext.Courses
            .AnyAsync(c => c.Id == parsedCourseId);
        if (!courseExists)
        {
            ModelState.AddModelError(nameof(LessonEditInputModel.CourseId), "Избраният курс не съществува.");
            model.Courses = await courseService.GetCourseOptionsAsync();
            return View(model);
        }

        courseId = parsedCourseId;
    }

    lessonToChange.Name = model.Name;
    lessonToChange.Content = model.Content;
    lessonToChange.OrderIndex = model.OrderIndex;
    lessonToChange.CourseId = courseId;
    lessonToChange.Target = model.Target;
    
    var grammar = lessonToChange.LessonSections
        .FirstOrDefault(ls => ls.Type == "grammar") ;
    
    if (grammar == null)
    {
        lessonToChange.LessonSections
            .Add(new LessonSection
            {
                Type = "grammar",
                Content = model.Grammar,
                OrderIndex = 1
            });
    }
    else
        grammar.Content = model.Grammar;
    
    var exercise = lessonToChange.LessonSections
        .FirstOrDefault(ls => ls.Type == "exercise");

    if (exercise == null)
    {
        lessonToChange.LessonSections
            .Add(new LessonSection
            {
                Type = "exercise",
                Content = model.Exercise,
                OrderIndex = 2
            });
    }
    else
        exercise.Content = model.Exercise;
    
    await dbcontext.SaveChangesAsync();
    
    TempData["SuccessMessage"] = $"Успешно редактирахте {lessonToChange.Name}.";
    return RedirectToAction(nameof(Details), new { id = lessonId });
}

}