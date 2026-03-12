using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using LerningApp.Web.ViewModels.ListeningExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.ApplicationConstants;
namespace LerningApp.Controllers;

[Authorize]
public class ListeningExerciseController(LerningAppContext dbContext,
    ITeacherService teacherService,
    IFileService fileService) :BaseController
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
            return RedirectToAction("Details", "Lesson", new { id = lessonId });
        }
       
        var model = new CreateListeningExerciseViewModel()
        {
            LessonId = lessonId
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateListeningExerciseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        if (string.IsNullOrWhiteSpace(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return RedirectToAction("Index", "Home");
        }

        Lesson? lesson = await dbContext.Lessons.FindAsync(lessonId);
        
        if (lesson == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        string userId = User.GetUserId()!;
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return RedirectToAction("Details", "Lesson", new { id = lessonId });
        }
        
        if (model.AudioFile.Length == 0)
        {
            ModelState.AddModelError(nameof(model.AudioFile), "Audio file is required.");
            return View(model);
        }

        string audioPath = string.Empty;
        if (model.AudioFile.Length > 0)
        {
            string[] allowedExtensions = [".mp3",".wav",".ogg",".m4a"];
            long maxSize = MaxFileSize;

            if (!fileService.IsFileValid(model.AudioFile, allowedExtensions, maxSize))
            {
                ModelState.AddModelError(nameof(model.AudioFile), "Please upload a valid audio file (.mp3, .wav, .ogg, .m4a) up to 5 MB.");
                return View(model);
            }

            string extension = Path.GetExtension(model.AudioFile.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            audioPath = await fileService.UploadFileAsync(model.AudioFile, DefaultaListeningExerciseAudiosPath, uniqueFileName);
        }
        
        ListeningExercise exercise = new()
        {
            LessonId = lessonId,
            DifficultyLevel = model.DifficultyLevel,
            PublisherId = teacherId.Value,
            AudioPath = audioPath,
        };
        
        List<ListeningQuestion> questions = new List<ListeningQuestion>();

        if (model.Questions.Count == 0)
        {
            ModelState.AddModelError(nameof(model.Questions), "Add at least one question for the exercise.");
            return View(model);
        }
        
        foreach (var question in model.Questions)
        {
            if (!string.IsNullOrWhiteSpace(question.QuestionText))
            {
                
                ListeningQuestion newQuestion = new ListeningQuestion()
                {
                    Question = question.QuestionText,
                    PublisherId = teacherId.Value,
                    ListeningExerciseId = exercise.Id,
                };
                
                var filledOptions = question.Options?
                    .Where(op => !string.IsNullOrWhiteSpace(op.AnswerText))
                    .ToList();
                
                if (filledOptions.Count() <= 1)
                {
                    ModelState.AddModelError(nameof(model.Questions), "Add at least two options for the question.");
                    return View(model);
                }

                int selectedCorrectOptionsCount = filledOptions
                    .Count(op => op.IsCorrect == true);
                
                if (selectedCorrectOptionsCount != 1)
                {
                    ModelState.AddModelError(nameof(model.Questions), "Choose one correct option for the question.");
                    return View(model);
                }
                
                List<ListeningExerciseOption> options = new List<ListeningExerciseOption>();
                foreach (var option in question.Options)
                {
                    if (!string.IsNullOrWhiteSpace(option.AnswerText) )
                    {
                        ListeningExerciseOption newOption = new ListeningExerciseOption()
                        {
                            Answer = option.AnswerText,
                            isCorrect = option.IsCorrect,
                            OrderIndex = option.OrderIndex,
                            ListeningQuestionId = newQuestion.Id,
                        };
                        options.Add(newOption);
                    }
                }
                newQuestion.Options = options;
                questions.Add(newQuestion);
            }
        }
        
        if (questions.Count == 0)
        {
            ModelState.AddModelError(nameof(model.Questions), "Add at least one valid question.");
            return View(model);
        }
        
        exercise.Questions = questions;
        
        await dbContext.ListeningExercises.AddAsync(exercise);
        await dbContext.SaveChangesAsync();
        
        return RedirectToAction("Details", "Lesson", new { id = model.LessonId });
    }
}