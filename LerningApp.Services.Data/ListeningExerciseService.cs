using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.ListeningExercise;
using LerningApp.Web.ViewModels.ListeningExercise.DTOs;
using Microsoft.EntityFrameworkCore;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data;

public class ListeningExerciseService(
    IRepository<Lesson, Guid> lessonRepository,
    IRepository<ListeningExercise, Guid> listeningExerciseRepository,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository,
    IUserExerciseProgressService userExerciseProgressService,
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
            return ServiceResult.Fail("Add at least one valid question.", nameof(model.Questions));
        }

        exercise.Questions = questions;
        listeningExerciseRepository.Add(exercise);
        await listeningExerciseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<(List<ListeningQuestionCheckResultDTO> Results, bool IsCompleted)> CheckListeningExerciseAnswer(CheckListeningExerciseInputModel model, string userId)
    {
        List<ListeningQuestionCheckResultDTO> results = new();
        if (!Guid.TryParse(userId, out var userGuidId))
        {
            return (results, false);
        }
        
        if (!Guid.TryParse(model.LessonId, out var lessonGuidId))
        {
            return (results, false);

        }
        
        var isTeacher = await teacherService.IsUserTeacherAsync(userId.ToString());
        var isUnlocked = isTeacher || await userLessonProgressRepository
            .GetAllAttached()
            .AnyAsync(x => x.UserId == userGuidId && x.LessonId == lessonGuidId && x.IsUnlocked);

        if (!isUnlocked)
        {
            return (results, false);

        }
        
        if (!Guid.TryParse(model.ExerciseId, out var exerciseGuidId))
        {
            return (results, false);
        }
        
        var exercise = await listeningExerciseRepository
            .GetAllAttached()
            .Include(e => e.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == exerciseGuidId && e.LessonId == lessonGuidId);

        if (exercise == null)
        {
            return (results, false);
        }
        
        var answerMap = model
            .Answers
            .ToDictionary(a => a.QuestionId, a => a.SelectedAnswer);
        
        var correct = 0;
        
        foreach (var q in exercise.Questions)
        {
            var correctOption = q.Options
                .FirstOrDefault(o => o.isCorrect);
            if (correctOption == null)
            {
                return (null, false);
            }

            string correctAnswer = correctOption.Answer;
            bool isCorrect = answerMap.TryGetValue(q.Id, out var selected) &&
                            selected == correctAnswer;

            if (isCorrect)
            {
                correct++;
            }
            
            results.Add(new ListeningQuestionCheckResultDTO()
            {
                IsCorrect = isCorrect,
                QuestionId = q.Id.ToString()
            });
        }
        
        var total = exercise.Questions.Count;
        var isCompleted = total > 0 && correct == total;

        if (isCompleted)
        {
            await userExerciseProgressService.CompleteExerciseAsync(userGuidId, exerciseGuidId);
        }
        
        return (results, isCompleted);
    }
}