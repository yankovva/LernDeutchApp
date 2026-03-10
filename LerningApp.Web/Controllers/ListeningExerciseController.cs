using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.ListeningExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

[Authorize]
public class ListeningExerciseController(LerningAppContext dbContext, ITeacherService teacherService) :BaseController
{
    [HttpGet]
    public async Task<IActionResult> Create(string lessonId)
    {
        string userId = User.GetUserId()!;
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return RedirectToAction("Index", "Home");
        }
        Lesson? lesson = await dbContext.Lessons.FindAsync(lessonGuid);

        if (lesson == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return RedirectToAction("Index", "Home");
        }
       
        var model = new CreateListeningExerciseViewModel()
        {
            LessonId = lessonId
        };
        return View(model);
    }
    
}