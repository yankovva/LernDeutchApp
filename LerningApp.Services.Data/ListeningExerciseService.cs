using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.ListeningExercise;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data;

public class ListeningExerciseService(
    IRepository<Lesson, Guid> lessonRepository,
    IRepository<ListeningExercise, Guid> listeningExerciseRepository,
    IRepository<ListeningQuestion, Guid> listeningQuestionRepository,
    ITeacherService teacherService,
    IFileService fileService) : IListeningExerciseService
{
    public async Task<ServiceResultT<CreateListeningExerciseViewModel>> CreateGetListeningExercise(string lessonId,
        string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<CreateListeningExerciseViewModel>.Fail(LessonNotFoundMessage);
        }

        Lesson? lesson = await lessonRepository.GetByIdAsync(lessonGuid);

        if (lesson == null)
        {
            return ServiceResultT<CreateListeningExerciseViewModel>.Fail(LessonNotFoundMessage);
        }

        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResultT<CreateListeningExerciseViewModel>.Fail(AccessDeniedMessage);
        }

        var model = new CreateListeningExerciseViewModel()
        {
            LessonId = lessonId
        };

        return ServiceResultT<CreateListeningExerciseViewModel>.Success(model);

    }

    public async Task<ServiceResult> CreatePostListeningExercise(CreateListeningExerciseViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return ServiceResult.Fail(LessonNotFoundMessage);
        }

        Lesson? lesson = await lessonRepository.GetByIdAsync(lessonId);

        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage);
        }

        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null || lesson.PublisherId != teacherId)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        if (model.AudioFile.Length == 0)
        {
            return ServiceResult.Fail("Audio file is required.", nameof(model.AudioFile));
        }

        string audioPath = string.Empty;
        if (model.AudioFile.Length > 0)
        {
            string[] allowedExtensions = [".mp3", ".wav", ".ogg", ".m4a"];
            long maxSize = MaxFileSize;

            if (!fileService.IsFileValid(model.AudioFile, allowedExtensions, maxSize))
            {
                return ServiceResult.Fail("Please upload a valid audio file (.mp3, .wav, .ogg, .m4a) up to 5 MB.",
                    nameof(model.AudioFile));
            }

            string extension = Path.GetExtension(model.AudioFile.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            audioPath = await fileService.UploadFileAsync(model.AudioFile, DefaultaListeningExerciseAudiosPath,
                uniqueFileName);
        }

        ListeningExercise exercise = new()
        {
            LessonId = lessonId,
            DifficultyLevel = model.DifficultyLevel,
            PublisherId = teacherId.Value,
            AudioPath = $"/{audioPath}",
        };

        List<ListeningQuestion> questions = new List<ListeningQuestion>();

        if (model.Questions.Count == 0)
        {
            return ServiceResult.Fail("Add at least one question for the exercise.", nameof(model.Questions));
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
                    .ToList() ?? new List<CreateListeningQuestionOptionInputModel>();

                if (filledOptions.Count <= 1)
                {
                    return ServiceResult.Fail("Add at least two options for the question.", nameof(model.Questions));
                }

                int selectedCorrectOptionsCount = filledOptions
                    .Count(op => op.IsCorrect == true);

                if (selectedCorrectOptionsCount != 1)
                {
                    return ServiceResult.Fail("Choose one correct option for the question.", nameof(model.Questions));
                }

                List<ListeningExerciseOption> options = new List<ListeningExerciseOption>();
                foreach (var option in question.Options)
                {
                    if (!string.IsNullOrWhiteSpace(option.AnswerText))
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
            //ModelState.AddModelError(nameof(model.Questions), "Add at least one valid question.");
            return ServiceResult.Fail("Add at least one valid question.", nameof(model.Questions));
        }

        exercise.Questions = questions;
        listeningExerciseRepository.Add(exercise);
        await listeningExerciseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<(bool isCorrect, string correctAnswer)?> CheckListeningExerciseAnswer(string exerciseId, string selectedAnswer)
    {
        if (!Guid.TryParse(exerciseId, out var queId))
        {
            return null;
        }
        
        var question = await listeningQuestionRepository
            .GetAllAttached()
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == queId);

        if (question == null)
        {
           return null;
        }
        
        bool isCorrect;
        var correctAnswer = question
            .Options
            .FirstOrDefault(op => op.isCorrect);
        
        if (correctAnswer == null)
        {
            return null;
        }
        
        if (correctAnswer.Answer == selectedAnswer)
        {
            isCorrect = true;
        } else
            isCorrect = false;
        
        return (isCorrect, correctAnswer.Answer);
    }
}