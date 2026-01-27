using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.Lesson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

public class LessonController  : Controller
{
    private readonly LerningAppContext _dbcontext;

    public LessonController(LerningAppContext dbcontext)
    {
        this._dbcontext = dbcontext;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        
        IEnumerable<LessonIndexViewModel> lessons = this._dbcontext.Lessons
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
            .ToList();
       return View(lessons);
    }
}