using LerningApp.Common;
using LerningApp.Contracts.ListeningExerciseDtos;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.ListeningExercise;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.ApplicationConstants;
using static LerningApp.Common.EntityErrorMessages.Exercise;
using static LerningApp.Common.Enums;

namespace LerningApp.Services.Data;

public class ListeningExerciseService(
    IRepository<Lesson, Guid> lessonRepository,
    IRepository<ListeningExercise, Guid> listeningExerciseRepository,
    IRepository<ListeningQuestion,Guid> questionRepository,
    IRepository<ListeningExerciseOption, Guid> optionRepository,
    IRepository<UserLessonProgress, Guid> userLessonProgressRepository,
    IUserExerciseProgressService userExerciseProgressService,
    ITeacherService teacherService,
    IFileService fileService,
    UserManager<ApplicationUser> userManager) : IListeningExerciseService
{
    public async Task<ServiceResultT<CreateListeningExerciseViewModel>> CreateGetListeningExercise(string lessonId,
        string userId)
    {
        if (string.IsNullOrWhiteSpace(lessonId) || !Guid.TryParse(lessonId, out Guid lessonGuid))
        {
            return ServiceResultT<CreateListeningExerciseViewModel>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository.GetByIdAsync(lessonGuid);

        if (lesson == null)
        {
            return ServiceResultT<CreateListeningExerciseViewModel>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResultT<CreateListeningExerciseViewModel>.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
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
            return ServiceResult.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }

        Lesson? lesson = await lessonRepository.GetByIdAsync(lessonId);

        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }

        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }

        if (model.AudioFile.Length == 0)
        {
            return ServiceResult.Fail("Audio file is required.",ServiceErrorType.Validation, nameof(model.AudioFile));
        }

        string audioPath = string.Empty;
        if (model.AudioFile.Length > 0)
        {
            string[] allowedExtensions = [".mp3", ".wav", ".ogg", ".m4a"];
            long maxSize = MaxFileSize;

            if (!fileService.IsFileValid(model.AudioFile, allowedExtensions, maxSize))
            {
                return ServiceResult.Fail("Please upload a valid audio file (.mp3, .wav, .ogg, .m4a) up to 5 MB.", ServiceErrorType.Validation,
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
            AudioPath = audioPath,
        };

        List<ListeningQuestion> questions = new List<ListeningQuestion>();

        var filledQuestions = model.Questions
            .Where(q => !string.IsNullOrWhiteSpace(q.QuestionText))
            .ToList();

        if (!filledQuestions.Any())
        {
            return ServiceResult.Fail("Add at least one question for the exercise.", ServiceErrorType.General, nameof(model.Questions));
        }

        foreach (var question in filledQuestions)
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
                    return ServiceResult.Fail("Add at least two options for the question.", ServiceErrorType.General,nameof(model.Questions));
                }

                int selectedCorrectOptionsCount = filledOptions
                    .Count(op => op.IsCorrect == true);

                if (selectedCorrectOptionsCount != 1)
                {
                    return ServiceResult.Fail("Choose one correct option for the question.", ServiceErrorType.General,nameof(model.Questions));
                }

                List<ListeningExerciseOption> options = new List<ListeningExerciseOption>();
                foreach (var option in filledOptions)
                {
                    if (!string.IsNullOrWhiteSpace(option.AnswerText))
                    {
                        ListeningExerciseOption newOption = new ListeningExerciseOption()
                        {
                            Answer = option.AnswerText,
                            IsCorrect = option.IsCorrect,
                            OrderIndex = option.OrderIndex,
                            ListeningQuestionId = newQuestion.Id,
                        };
                        options.Add(newOption);
                    }
                }

                newQuestion.Options = options;
                questions.Add(newQuestion);
            
        }

        if (questions.Count == 0)
        {
            return ServiceResult.Fail("Add at least one valid question.", ServiceErrorType.General,nameof(model.Questions));
        }

        exercise.Questions = questions;
        listeningExerciseRepository.Add(exercise);
        await listeningExerciseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<List<ListeningQuestionCheckResultDto>>> CheckListeningExerciseAnswer(CheckListeningExerciseInputDto dto, string userId)
    {
        List<ListeningQuestionCheckResultDto> results = new();
        if (!Guid.TryParse(userId, out var userGuidId))
        {
            return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        if (!Guid.TryParse(dto.LessonId, out var lessonGuidId))
        {
            return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        var isTeacher = await teacherService.IsUserTeacherAsync(userId.ToString());
        var isUnlocked = isTeacher || await userLessonProgressRepository
            .GetAllAttached()
            .AnyAsync(x => x.UserId == userGuidId && x.LessonId == lessonGuidId && x.IsUnlocked);

        if (!isUnlocked)
        {
            return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        if (!Guid.TryParse(dto.ExerciseId, out var exerciseGuidId))
        {
            return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        var exercise = await listeningExerciseRepository
            .GetAllAttached()
            .Include(e => e.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == exerciseGuidId && e.LessonId == lessonGuidId);

        if (exercise == null)
        {
            return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
        }
        
        var answerMap = dto
            .Answers
            .ToDictionary(a => a.QuestionId, a => a.SelectedAnswer);
        
        var correct = 0;
        
        foreach (var q in exercise.Questions)
        {
            var correctOption = q.Options
                .FirstOrDefault(o => o.IsCorrect);
            if (correctOption == null)
            {
                return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
            }

            string correctAnswer = correctOption.Answer;
            bool isCorrect = answerMap.TryGetValue(q.Id, out var selected) &&
                            selected == correctAnswer;

            if (isCorrect)
            {
                correct++;
            }
            
            results.Add(new ListeningQuestionCheckResultDto()
            {
                IsCorrect = isCorrect,
                QuestionId = q.Id.ToString()
            });
        }
        
        var total = exercise.Questions.Count;
        var isCompleted = total > 0 && correct == total;

        if (isCompleted && !isTeacher)
        {
            var result = await userExerciseProgressService.CompleteExerciseAsync(userGuidId, exercise.Id);
            if (result.Result == false)
            {
                return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Fail(LessonNotFoundMessage, ServiceErrorType.NotFound);
            }
        }
        
        return ServiceResultT<List<ListeningQuestionCheckResultDto>>.Success(results);
    }

    public async Task<ServiceResult> SoftDeleteExerciseAsync(string exerciseId, string userId)
    {
        if (!Guid.TryParse(exerciseId, out var exerciseGuidId))
        {
            return ServiceResult.Fail(InvalidExerciseIdMessage, ServiceErrorType.NotFound);
        }

        var exercise = await listeningExerciseRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == exerciseGuidId);

        if (exercise == null)
        {
            return ServiceResult.Fail(InvalidExerciseIdMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        exercise.IsDeleted = true;

        await listeningExerciseRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<EditListeningExerciseViewModel>> GetEditListeningExercise(string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid exerciseId))
        {
            return ServiceResultT<EditListeningExerciseViewModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }

        ListeningExercise? exercise = await listeningExerciseRepository
            .GetAllAttached()
            .Include(e => e.Lesson)
            .Include(e => e.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == exerciseId);
        
        if (exercise == null)
        {
            return ServiceResultT<EditListeningExerciseViewModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResultT<EditListeningExerciseViewModel>.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }
        
        var model = new EditListeningExerciseViewModel()
        {
            Id = exercise.Id.ToString(),
            LessonId = exercise.LessonId.ToString(),
            DifficultyLevel = exercise.DifficultyLevel,
            AudioPath = exercise.AudioPath,
            Questions = exercise.Questions
                .Where(q => q.IsDeleted == false)
                .Select(q => new EditListeningQuestionInputModel()
                {
                    QuestionText = q.Question,
                    Id = q.Id.ToString(),
                    ExerciseId = exercise.Id.ToString(),
                    Options = q.Options
                        .Select(o => new EditListeningQuestionOptionInputModel()
                        {
                            IsCorrect = o.IsCorrect,
                            Id = o.Id.ToString(),
                            AnswerText = o.Answer,
                            OrderIndex = o.OrderIndex,
                        }).ToList()
                    
                }).ToList()
        };
        
        return ServiceResultT<EditListeningExerciseViewModel>.Success(model);
    }

    public async Task<ServiceResult> PostEditListeningExercise(EditListeningExerciseViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.Id) || !Guid.TryParse(model.Id, out Guid exerciseId))
        {
            return ServiceResult.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }

        ListeningExercise? exercise = await listeningExerciseRepository
            .GetAllAttached()
            .Include(e => e.Lesson)
            .Include(e => e.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == exerciseId);
        
        if (exercise == null)
        {
            return ServiceResult.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await userManager.IsInRoleAsync(user, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage,ServiceErrorType.AccessDenied);
        }
        
        string? oldAudioPathToDelete = null;
        
        string newAudioPath = string.Empty;

        if (model.AudioFile != null && model.AudioFile.Length > 0)
        {
            string[] allowedExtensions = [".mp3", ".wav", ".ogg", ".m4a"];
            long maxSize = MaxFileSize;

            if (!fileService.IsFileValid(model.AudioFile, allowedExtensions, maxSize))
            {
                return ServiceResult.Fail(
                    "Please upload a valid audio file (.mp3, .wav, .ogg, .m4a) up to 5 MB.",
                    ServiceErrorType.Validation,
                    nameof(model.AudioFile));
            }

            string extension = Path.GetExtension(model.AudioFile.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";

            newAudioPath = await fileService.UploadFileAsync(
                model.AudioFile,
                DefaultaListeningExerciseAudiosPath,
                uniqueFileName);

            oldAudioPathToDelete = exercise.AudioPath;
            exercise.AudioPath = newAudioPath;
        }
        
        exercise.DifficultyLevel = model.DifficultyLevel;
        await listeningExerciseRepository.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(oldAudioPathToDelete))
        {
            fileService.DeleteFile(oldAudioPathToDelete);
        }
        
        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<EditListeningQuestionInputModel>> GetEditListeningQuestion(string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid questionId))
        {
            return ServiceResultT<EditListeningQuestionInputModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }

        ListeningQuestion? question = await questionRepository
            .GetAllAttached()
            .Include(q => q.Options)
            .Include(q => q.ListeningExercise)
            .FirstOrDefaultAsync(q => q.Id == questionId);
        
        if (question == null)
        {
            return ServiceResultT<EditListeningQuestionInputModel>.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await userManager.IsInRoleAsync(user, AdminRole);
        
        if (!isAdmin && (teacherId == null || question.PublisherId != teacherId))
        {
            return ServiceResultT<EditListeningQuestionInputModel>.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        var model = new EditListeningQuestionInputModel()
        {
            Id = question.Id.ToString(),
            ExerciseId = question.ListeningExerciseId.ToString(),
            QuestionText = question.Question,
            Options = question.Options
                .Select(op => new EditListeningQuestionOptionInputModel()
                {
                    IsCorrect = op.IsCorrect,
                    AnswerText = op.Answer,
                    OrderIndex = op.OrderIndex,
                    Id = op.Id.ToString(),
                }).ToList()
        };

        model.CorrectOptionIndex = model.Options.FindIndex(op => op.IsCorrect);

        return ServiceResultT<EditListeningQuestionInputModel>.Success(model);
    }

    public async Task<ServiceResult> PostEditListeningQuestion(EditListeningQuestionInputModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.Id) || !Guid.TryParse(model.Id, out Guid questionId))
        {
            return ServiceResult.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }

        ListeningQuestion? question = await questionRepository
            .GetAllAttached()
            .Include(q => q.Options)
            .Include(q => q.ListeningExercise)
            .FirstOrDefaultAsync(q => q.Id == questionId);
        
        if (question == null)
        {
            return ServiceResult.Fail(ExerciseNotFoundMessage, ServiceErrorType.NotFound);
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await userManager.IsInRoleAsync(user, AdminRole);
        
        if (!isAdmin && (teacherId == null || question.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        for (int i = 0; i < model.Options.Count; i++)
        {
            model.Options[i].IsCorrect = i == model.CorrectOptionIndex;
        }

        var modelOptions = model.Options
            .OrderBy(op => op.OrderIndex)
            .Where(op => !string.IsNullOrWhiteSpace(op.AnswerText))
            .ToList();

        if (modelOptions.Any(mo => string.IsNullOrWhiteSpace(mo.AnswerText)))
        {
            return ServiceResult.Fail(
                "There can not be an empty option.",
                ServiceErrorType.Validation,
                nameof(model.Options));
        }

        if (modelOptions.Count != question.Options.Count)
        {
                return ServiceResult.Fail(
                    "Editing the number of questions is not supported here.",
                    ServiceErrorType.General,
                    nameof(model.Options));
        }
        
        if (modelOptions.Count <= 1)
        {
            return ServiceResult.Fail(
                "Add at least two options for the question.",
                ServiceErrorType.General,
                nameof(model.Options));
        }

        int selectedCorrectOptionsCount = modelOptions
            .Count(op => op.IsCorrect);
        
        if (selectedCorrectOptionsCount != 1)
        {
            return ServiceResult.Fail(
                "Choose one correct option for the question.",
                ServiceErrorType.General,
                nameof(model.Options));
        }

        var dbOptions = question.Options
            .OrderBy(op => op.OrderIndex)
            .ToList();
        
        for (int j = 0; j < question.Options.Count; j++)
        {
            dbOptions[j].Answer = model.Options[j].AnswerText!;
            dbOptions[j].IsCorrect = model.Options[j].IsCorrect;
        }
        
        question.Question = model.QuestionText;
        await questionRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteOptionAsync(DeleteListeningOptionViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.Id) || !Guid.TryParse(model.Id, out Guid optionId))
        {
            return ServiceResult.Fail(InvalidOptionMessage, ServiceErrorType.NotFound);
        }
        
        ListeningExerciseOption? option = await optionRepository
            .GetByIdAsync(optionId);
        
        if (option == null)
        {
            return ServiceResult.Fail(InvalidOptionMessage, ServiceErrorType.NotFound);
        }
        
        optionRepository.DeleteByEntity(option);
        await optionRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SoftDeleteQuestionAsync(string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid optionId))
        {
            return ServiceResult.Fail(InvalidQuestionMessage, ServiceErrorType.NotFound);
        }
        
        ListeningQuestion? question = await questionRepository
            .GetByIdAsync(optionId);
        
        if (question == null)
        {
            return ServiceResult.Fail(InvalidQuestionMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await userManager.IsInRoleAsync(user, AdminRole);
        
        if (!isAdmin && (teacherId == null || question.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }
        
        question.IsDeleted = true;
        await questionRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> CreateQuestionAsync(AddListeningQuestionToExerciseViewModel model, string userId)
    {
        if (string.IsNullOrWhiteSpace(model.ExerciseId) || !Guid.TryParse(model.ExerciseId, out Guid exerciseId))
        {
            return ServiceResult.Fail(InvalidQuestionMessage, ServiceErrorType.NotFound);
        }
        
        ListeningExercise? exercise = await listeningExerciseRepository
            .GetByIdAsync(exerciseId);
        
        if (exercise == null)
        {
            return ServiceResult.Fail(InvalidQuestionMessage, ServiceErrorType.NotFound);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await userManager.IsInRoleAsync(user, AdminRole);
        
        if (!isAdmin && (teacherId == null || exercise.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage, ServiceErrorType.AccessDenied);
        }

        ListeningQuestion newQuestion = new()
        {
            Id = Guid.NewGuid(),
            Question = model.QuestionText,
            ListeningExerciseId = exerciseId,
            PublisherId = teacherId.Value
        };
        
        var filledOptions = model.Options
            .Where(o => !string.IsNullOrWhiteSpace(o.AnswerText))
            .ToList();
        
        if (filledOptions.Count <= 1)
        {
            return ServiceResult.Fail("Add at least two options for the question.", ServiceErrorType.General);
        }
        
        var newOptions = new List<ListeningExerciseOption>();
        foreach (var option in filledOptions)
        {
            var newOption = new ListeningExerciseOption
            {
                Id = Guid.NewGuid(),
                Answer = option.AnswerText,
                OrderIndex = option.OrderIndex,
                ListeningQuestionId = newQuestion.Id,
            };
            
            if (option.OrderIndex == model.CorrectOptionIndex)
            {
                newOption.IsCorrect = true;
            }
            newOptions.Add(newOption);
        }
        newQuestion.Options = newOptions;
        questionRepository.Add(newQuestion);
        await questionRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }
}
