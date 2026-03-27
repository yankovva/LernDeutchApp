using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.LessonInterfaces;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.ListeningExercise;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using LerningApp.Web.ViewModels.TranslationExercise;
using LerningApp.Web.ViewModels.UserLessonProgress;

using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Lesson;


namespace LerningApp.Services.Data.LessonServices;

public class LessonQueryService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<MultipleChoiceExercise, Guid> multipleExerciseRepository,
    IRepository<TranslationExercise, Guid> translationExersiceRepository,
    IRepository<ListeningExercise, Guid> listeningExerciseRepository,
    IRepository<Course, Guid> courseRepository,
    IUserLessonProgressService userLessonProgressService,
    ITeacherService teacherService,
    IUserExerciseProgressService userExerciseProgressService) : ILessonQueryService
{
    public async Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync()
    {
        IEnumerable<LessonIndexViewModel> lessons =  await lessonRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(l => l.Course)
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LessonIndexViewModel
            {
                Id = l.Id.ToString(),
                Name = l.Name,
                Publisher = l.PublisherId.ToString(),
                CourseId = l.CourseId != null ? l.CourseId.ToString() : null,
                CourseName = l.Course != null ? l.Course.Name : null,
                LevelName = l.Course != null ? l.Course.Level.Name : null,
                CreatedAt = l.CreatedAt.ToString("dd.MM.yyyy"),
            })
            .ToListAsync();
        
        return lessons;
    }

    public async Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResultT<LessonContentViewModel>.Fail(InvalidLessonIdMessage);
        }
        
        Lesson? lesson = await lessonRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(l => l.Course)
            .Include(lesson => lesson.VocabularyCards)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return ServiceResultT<LessonContentViewModel>.Fail(LessonNotFoundMessage);
        }

        List<IndexListeningExerciseViewModel> listeningExercisesViewModels = await listeningExerciseRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(ex => ex.LessonId == lessonId)
            .OrderBy(ex => ex.DifficultyLevel)
            .Select(ex => new IndexListeningExerciseViewModel
            {
                Id = ex.Id.ToString(),
                AudioPath = ex.AudioPath,
                Qestions = ex.Questions
                    .Select(q => new IndexListeningQestionViewModel()
                    {
                        Question = q.Question,
                        Id = q.Id.ToString(),
                        Options = q.Options
                            .Select(op => new IndexListeningOptionsViewModel()
                            {
                                AnswerText = op.Answer,
                                IsCorrect = op.IsCorrect
                            }).ToList()
                    }).ToList()
            }).ToListAsync();

        List<IndexMultipleChoiceExerciseViewModel> multipleChoiceExerciseViewModels = await multipleExerciseRepository
            .GetAllAttached()
            .Where(ex => ex.LessonId == lessonId)
            .OrderBy(ex => ex.DifficultyLevel)
            .Select(ex => new IndexMultipleChoiceExerciseViewModel()
            {
                Id = ex.Id.ToString(),
                Question = ex.Question,
                Options = ex.Options
                    .Select(op => new MultipleChoiceOptionsIndexViewModel()
                    {
                        IsCorrect = op.IsCorrect,
                        Answer = op.Answer,
                        OrderIndex = op.OrderIndex
                    }).ToList()
            }).ToListAsync();

        List<IndexTranslationExerciseViewModel> translationExerciseViewModels = await translationExersiceRepository
            .GetAllAttached()
            .Where(ex => ex.LessonId == lessonId)
            .OrderBy(ex => ex.DifficultyLevel)
            .Select(ex => new IndexTranslationExerciseViewModel()
            {
                Id = ex.Id.ToString(),
                GermanSentence = ex.GermanSentence,
                EnglishSentence = ex.EnglishSentence,
                BulgarianSentence = ex.BulgarianSentence,
            })
            .ToListAsync();
        
        var userProgressResult = await userLessonProgressService
            .GetUserLessonProgress(lessonId, userId);
 
        bool isUserTeacher = await teacherService
            .IsUserTeacherAsync(userId);
        
        if (userProgressResult.Result == false && !isUserTeacher)
        {
            return ServiceResultT<LessonContentViewModel>.Fail(userProgressResult.Message ?? "Invalid operation.");
        }
        
        if (userProgressResult.Data != null && !userProgressResult.Data.IsUnlocked && !isUserTeacher)
        {
            return ServiceResultT<LessonContentViewModel>.Fail("Lesson is locked.");
        }
        
        await userExerciseProgressService.CreateUserExerciseProgress(listeningExercisesViewModels, x => x.Id, userId, lessonId, Enums.ExerciseType.ListeningExercise);
        await userExerciseProgressService.CreateUserExerciseProgress(multipleChoiceExerciseViewModels, x => x.Id, userId, lessonId, Enums.ExerciseType.MultipleChoiceExercise);
        await userExerciseProgressService.CreateUserExerciseProgress(translationExerciseViewModels, x => x.Id, userId, lessonId, Enums.ExerciseType.TranslationExercise);
       
        
        LessonContentViewModel model = new LessonContentViewModel()
        {
            Id = lesson.Id.ToString(),
            Name = lesson.Name,
            CourseId = lesson.CourseId?.ToString(),
            Content = lesson.Content,
            WordCount = lesson.VocabularyCards.Count(),
            PublisherId = lesson.PublisherId.ToString(),
            UserLessonProgress = userProgressResult.Data ?? new IndexUserLessonProgressViewModel(),
            OrderIndex = lesson.OrderIndex,
            CourseName = lesson.Course != null ? lesson.Course.Name : "No course found.",
            Target = lesson.Target,
            MultipleChoiceExercises = multipleChoiceExerciseViewModels,
            TranslationExercises = translationExerciseViewModels,
            ListeningExercises = listeningExercisesViewModels
        };
        
        if (isUserTeacher)
        {
            model.UserLessonProgress = new()
            {
                IsUnlocked = true,
            };
        }
        
        return ServiceResultT<LessonContentViewModel>.Success(model);
    }

    public async Task<List<int>> GetAvailableOrderIndexes(string courseId)
    {
        if (string.IsNullOrEmpty(courseId) || !Guid.TryParse(courseId, out Guid courseGuidId))
        {
            return null;
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(l => l.LessonsForCourse)
            .FirstOrDefaultAsync(c => c.Id == courseGuidId);    
        
        if (course == null)
        {
            return null;
        }

        var usedIndexes = await lessonRepository
            .GetAllAttached()
            .Where(l => l.CourseId == courseGuidId)
            .Select(l => l.OrderIndex)
            .ToListAsync();

        if (usedIndexes.Count == 0)
        {
            return new List<int> { 1 };
        }

        int biggestIndex = usedIndexes.Max();
        List<int> availableIndexes = new List<int>();

        for (int i = 1; i <= biggestIndex + 1 ; i++)
        {
            if (!usedIndexes.Contains(i))
                availableIndexes.Add(i);
        }

        return availableIndexes;
    }
}