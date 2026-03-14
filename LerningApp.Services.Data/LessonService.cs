using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;
using LerningApp.Web.ViewModels.Lesson;
using LerningApp.Web.ViewModels.ListeningExercise;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using LerningApp.Web.ViewModels.TranslationExercise;

using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.EntityErrorMessages.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace LerningApp.Services.Data;

public class LessonService(IRepository<Lesson, Guid> lessonRepository,
    IRepository<Course, Guid> courseRepository,
    IRepository<MultipleChoiceExercise, Guid> multipleExerciseRepository,
    IRepository<TranslationExercise, Guid> translationExersiceRepository,
    IRepository<ListeningExercise, Guid> listeningExerciseRepository,
    IRepository<UserLessonProgress, Guid> lessonProgressRepository,
    IRepository<UserCourse, object> userCourseRepository,
    ITeacherService teacherService,
    IUserLessonProgressService userLessonProgressService) : ILessonService
{
    public async Task<IEnumerable<LessonIndexViewModel>> IndexGetLessonsAsync()
    {
        IEnumerable<LessonIndexViewModel> lessons =  await lessonRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(l => l.Course)
            .OrderBy(l => l.Name)
            .Select(l => new LessonIndexViewModel
            {
                Id = l.Id.ToString(),
                Name = l.Name,
                CourseId = l.CourseId != null ? l.CourseId.ToString() : null,
                CourseName = l.Course != null ? l.Course.Name : null,
                LevelName = l.Course != null ? l.Course.Level.Name : null,
                CreatedAt = l.CreatedAt.ToString("dd.MM.yyyy"),
            })
            .ToListAsync();
        
        return lessons;
    }

    public async Task<ServiceResultT<LessonContentViewModel>> GetLessonDetailsAsync(string id, string? userId)
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

        List<IndexListeningExerciseViewModel> listeningExercises = await listeningExerciseRepository
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
                                IsCorrect = op.isCorrect
                            }).ToList()
                    }).ToList()
            }).ToListAsync();

        List<IndexMultipleChoiceExerciseViewModel> multipleChoiceExerciseViewModels = await multipleExerciseRepository
            .GetAllAttached()
            .Where(ex => ex.LessonId == lessonId)
            .OrderBy(ex => ex.DifficultyLevel)
            .Select(ex => new IndexMultipleChoiceExerciseViewModel()
            {
                Question = ex.Question,
                Id = ex.Id.ToString(),
                CorrectAnswer = ex.CorrectAnswer,
                FirstWrongAnswer = ex.FirstWrongAnswer,
                SecondWrongAnswer = ex.SecondWrongAnswer,
                ThirdWrongAnswer = ex.ThirdWrongAnswer,
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
            }).ToListAsync();
        
        var userProgressResult = await userLessonProgressService
            .GetUserLessonProgress(lessonId, userId);

        if (userProgressResult.Result == false)
        {
            return ServiceResultT<LessonContentViewModel>.Fail(userProgressResult.Message ?? "Invalid operation.");
        }
        
        LessonContentViewModel model = new LessonContentViewModel()
        {
            Id = lesson.Id.ToString(),
            Name = lesson.Name,
            CourseId = lesson.CourseId.ToString(),
            Content = lesson.Content,
            WordCount = lesson.VocabularyCards.Count(),
            PublisherId = lesson.PublisherId.ToString(),
            UserLessonProgress = userProgressResult.Data,
            OrderIndex = lesson.OrderIndex,
            CourseName = lesson.Course != null ? lesson.Course.Name : "No course found.",
            Target = lesson.Target,
            MultipleChoiceExercises = multipleChoiceExerciseViewModels,
            TranslationExercises = translationExerciseViewModels,
            ListeningExercises = listeningExercises
        };
        
        return ServiceResultT<LessonContentViewModel>.Success(model);
    }

    public async Task<ServiceResultT<AddLessonToCourseViewModel>> GetAddLessonToCourseByIdAsync(string id,string userId)
    {
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (!isTeacher)
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail(AccessDeniedMessage);
        }
        
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail(InvalidLessonIdMessage);
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);

        if (lesson == null) 
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail(LessonNotFoundMessage);
        }

        AddLessonToCourseViewModel model = new AddLessonToCourseViewModel()
        {
            LessonId = lesson.Id.ToString(),
            LessonName = lesson.Name,
            SelectedCourseId = lesson.CourseId?.ToString().ToLower(),
            Courses = await courseRepository
                .GetAllAttached()
                .Select(c => new CourseCheckBoxItemInputModel
                {
                    CourseId = c.Id.ToString().ToLower(),
                    CourseName = c.Name,
                })
                .ToListAsync()
        };
        return ServiceResultT<AddLessonToCourseViewModel>.Success(model);
    }

    public async Task<ServiceResult> AddLessonToCourseAsync(AddLessonToCourseViewModel model, string userId)
    {
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (!isTeacher)
        {
            return ServiceResultT<AddLessonToCourseViewModel>.Fail(AccessDeniedMessage);
        }
        
        if (string.IsNullOrEmpty(model.LessonId) || !Guid.TryParse(model.LessonId, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage);
        }

        Lesson? lesson = await lessonRepository
            .GetByIdAsync(lessonId);

        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage);
        }
        
        //TODO: Delete UserLessonProgress records for enrolled users when a lesson is removed from a course (decide: hard delete vs soft delete).
        if (string.IsNullOrWhiteSpace(model.SelectedCourseId))
        {
            lesson.CourseId = null;
            await lessonRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }
           
        if (string.IsNullOrEmpty(model.SelectedCourseId) || !Guid.TryParse(model.SelectedCourseId, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage, nameof(model.SelectedCourseId));
        }

        Course? course = await courseRepository
            .GetAllAttached()
            .Include(c => c.CourseParticipants)
            .FirstOrDefaultAsync(c => c.Id == courseId);    
        
        if (course == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage,nameof(model.SelectedCourseId));
        }
        
        var userGuid = Guid.TryParse(userId, out Guid userGuidparsed) ? userGuidparsed : Guid.Empty;
        
        var isEnrolled = await userCourseRepository
            .GetAllAttached()
            .AnyAsync(x => x.UserId == userGuid && x.CourseId == courseId);

        if (isEnrolled)
        {
            var participantInCourse = await lessonProgressRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(l => l.LessonId == lessonId && l.UserId == userGuid);
            
            if (participantInCourse == null)
            {
                UserLessonProgress userLessonProgress = new UserLessonProgress()
                {
                    LessonId = lessonId,
                    UserId = userGuid,
                };

                await lessonProgressRepository.AddAsync(userLessonProgress);
            }
        }
        
        lesson.CourseId = courseId;

        await lessonRepository.SaveChangesAsync();
        
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> AddLessonAsync(AddLessonInputModel model, string userId)
    {
        Guid courseId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(model.CourseId))
        {
            if (!Guid.TryParse(model.CourseId, out  courseId))
            {
                return ServiceResult.Fail(InvalidCourseIdMessage, nameof(model.CourseId));
            }
            
            Course? course = await courseRepository
                .GetByIdAsync(courseId);
            
            if (course == null)
            {
                return ServiceResult.Fail(CourseNotFoundMessage, nameof(model.CourseId));
            }
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);

        if (teacherId == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        Lesson lesson = new Lesson
        {
            Name = model.Name,
            Content = model.Content,
            CourseId = courseId == Guid.Empty ? null : courseId,
            CreatedAt = DateTime.UtcNow,
            PublisherId = teacherId.Value,
            OrderIndex = model.OrderIndex,
            Target = model.Target,
        };
        
        await  lessonRepository.AddAsync(lesson);
        return ServiceResult.Success();
    }

    public async Task<ServiceResultT<LessonEditInputModel>> GetLessonEditInputModelAsync(string id, string userId)
    {
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (!isTeacher)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(AccessDeniedMessage);
        }
        
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResultT<LessonEditInputModel>.Fail(InvalidLessonIdMessage);
        }

        Lesson? lesson = await lessonRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(LessonNotFoundMessage);
        }

        var model = new LessonEditInputModel
        {
            Id = lesson.Id.ToString(),
            Name = lesson.Name,
            Content = lesson.Content,
            OrderIndex = lesson.OrderIndex,
            CourseId = lesson.CourseId?.ToString(),
            Target = lesson.Target,
            Courses = new List <CourseOptionsViewModel>{}
        };
        
        return ServiceResultT<LessonEditInputModel>.Success(model);
    }

    public async Task<ServiceResult> PostLessonEditInputModelAsync(LessonEditInputModel model, string id, string userId)
    {
        
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage);
        }

        Lesson? lessonToChange = await lessonRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lessonToChange == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage);
        }

        Guid? courseId = null;
        if (!string.IsNullOrWhiteSpace(model.CourseId))
        {
            if (!Guid.TryParse(model.CourseId, out var parsedCourseId))
                return ServiceResult.Fail(InvalidCourseIdMessage);

            var courseExists = await courseRepository.GetByIdAsync(parsedCourseId);
            if (courseExists == null)
                return ServiceResult.Fail(CourseNotFoundMessage);

            courseId = parsedCourseId;
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId != lessonToChange.PublisherId)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(AccessDeniedMessage);
        }
        
        lessonToChange.Name = model.Name;
        lessonToChange.Content = model.Content;
        lessonToChange.OrderIndex = model.OrderIndex;
        lessonToChange.CourseId = courseId;
        lessonToChange.Target = model.Target;
       
        await lessonRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SoftDeleteLessonAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid lessonId))
        {
            return ServiceResult.Fail(InvalidLessonIdMessage);
        }

        Lesson? lesson = await lessonRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(c => c.Id == lessonId);

        if (lesson == null)
        {
            return ServiceResult.Fail(LessonNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId != lesson.PublisherId)
        {
            return ServiceResultT<LessonEditInputModel>.Fail(AccessDeniedMessage);
        }
        
        lesson.IsDeleted = true;
        await lessonRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
    
}
