using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.ListeningExercise;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
    
using static LerningApp.Common.Enums;

namespace LerningApp.Controllers;

[Authorize]
public class ListeningExerciseController(IListeningExerciseService exerciseService) :BaseController
{
    //Done
    [HttpGet]
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Create(string lessonId)
    {
        string userId = User.GetUserId()!;
        var result = await exerciseService.CreateGetListeningExercise(lessonId, userId);
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
    [Authorize(Roles = "Admin, Teacher")]
    public async Task<IActionResult> Create(CreateListeningExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        string userId = User.GetUserId()!;
        
        var result = await exerciseService.CreatePostListeningExercise(model, userId);
        if (result.Result == false)
        {
            if (result.ErrorType == ServiceErrorType.Validation)
                ModelState.AddModelError(result.Field ?? string.Empty, result.Message!);
            else
                TempData["ErrorMessage"] = result.Message;
            return this.View(model);
        }

        return RedirectToAction("Details", "Lesson", new { id = model.LessonId });
    }
}
