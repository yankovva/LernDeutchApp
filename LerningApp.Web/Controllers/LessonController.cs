using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class LessonController  : BaseController
{
    private readonly LerningAppContext _dbcontext;

    public LessonController(LerningAppContext dbcontext)
    {
        this._dbcontext = dbcontext;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        
        IEnumerable<LessonIndexViewModel> lessons =  await this._dbcontext.Lessons
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
    public async Task<IActionResult> Content(string id)
    {
        Guid lessonId = Guid.Empty;

        bool isIdValid = IsGuidValid(id, ref lessonId);

        if (!isIdValid)
        {
            return this.RedirectToAction(nameof(this.Index));
        }
        
        Lesson? lesson = await this._dbcontext.Lessons
            .Include(l => l.Course)
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
            WordCount = lesson.VocabularyItems.Count(),
            OrderIndex = lesson.OrderIndex,
            CourseName = lesson.Course != null ? lesson.Course.Name : "No course found.",
        };

        return this.View(model);
    }
}