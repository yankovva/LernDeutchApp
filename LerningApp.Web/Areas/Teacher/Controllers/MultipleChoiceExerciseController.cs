using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Areas.Teacher.Controllers;

[Authorize(Roles = "Admin, Teacher")]
[Area("Teacher")]
public class MultipleChoiceExerciseController(IMultipleChoiceExerciseService exerciseService) : Controller
{
    //Done
    [HttpGet]
    public async Task<IActionResult> Create(string lessonId)
    { 
       string userId = User.GetUserId()!;
        var result = await exerciseService.GetCreateAsync(lessonId, userId!);
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return View(result.Data);
    }
    //Done
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMultipleChoiceExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        string userId = User.GetUserId()!;
        var result = await exerciseService.CreateAsync(model, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return View(model);
        }
       
        TempData["SuccessMessage"] = "Успешно създадохте упражнението";
        return RedirectToAction(nameof(Create), new { lessonId = model.LessonId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.GetEditMultipleChoice(id, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        return View(result.Data);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditMultipleExerciseViewModel model)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.PostEditMultipleChoice(model, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Manage", "Lesson", new { area = "Teacher", id = model.LessonId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(string id, string lessonId)
    {
        var userId = User.GetUserId()!;
        var result = await exerciseService
            .SoftDeleteExerciseAsync(id, userId);
        
        if (result.Result == false)
        {
            TempData["ErrorMessage"] = result.Message;
            return View(id);
        }
        
        TempData["SuccessMessage"] = "Successfully deleted exercise.";
        return RedirectToAction("Manage", "Lesson", new { area = "Teacher", id = lessonId });
    }
}