using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Controllers;

[Authorize]
public class MultipleChoiceExerciseController(LerningAppContext dbContext, UserManager<ApplicationUser> userManager) : Controller
{
    
    [HttpGet]
    public IActionResult Create(string lessonId)
    {
        var model = new CreateMultipleChoiceExerciseViewModel()
        {
          LessonId = lessonId
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMultipleChoiceExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        string currentUserId = userManager.GetUserId(User)!;
        
        Guid lessonId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(model.LessonId))
        {
            if (!Guid.TryParse(model.LessonId, out  lessonId))
            {
                ModelState.AddModelError("", "Невалиден урок.");
                return View(model);
            }
            
            Lesson? lesson = await dbContext
                .Lessons
                .FirstOrDefaultAsync(l=> l.Id == lessonId);
            
            if (lesson == null)
            {
                ModelState.AddModelError("", "Невалиден урок.");
                return View(model);
            }
        }

        MultipleChoiceExercise exercise = new MultipleChoiceExercise()
        {
            LessonId = lessonId,
            Question = model.Question,
            CorrectAnswer = model.CorrectAnswer,
            SecondWrongAnswer = model.SecondWrongAnswer ?? null,
            FirstWrongAnswer = model.FirstWrongAnswer,
            ThirdWrongAnswer = model.ThirdWrongAnswer ?? null,
            OrderIndex = model.OrderIndex,
            PublisherId = Guid.Parse(currentUserId),
        };
        
        await dbContext.MultipleChoiceExercises.AddAsync(exercise);
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Успешно създадохте упражнението";
        return RedirectToAction(nameof(Create), new { lessonId = model.LessonId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckMultipleChoice(string exerciseId, string lessonId, string selectedAnswer)
    {
        if (!Guid.TryParse(exerciseId, out var exId))
        {
            TempData["ErrorMessage"] = "Невалидно упражнение.";
            return RedirectToAction("Details", "Lesson" ,new { id = lessonId });
        }
        
        var exercise = await dbContext
            .MultipleChoiceExercises
            .FirstOrDefaultAsync(ex => ex.Id == Guid.Parse(exerciseId));

        if (exercise == null)
        {
            TempData["ErrorMessage"] = "Упражнението не е намерено.";
            return RedirectToAction("Details", "Lesson" ,new { id = lessonId });
        }
        
        bool isCorrect;
        if (exercise.CorrectAnswer == selectedAnswer)
        {
            isCorrect = true;
        } else
            isCorrect = false;
        
        return Json(new
        {
            isCorrect,
            correctAnswer = exercise.CorrectAnswer
        });
    }
}