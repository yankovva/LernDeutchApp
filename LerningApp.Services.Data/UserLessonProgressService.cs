using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.UserLessonProgress;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Course;

namespace LerningApp.Services.Data;

public class UserLessonProgressService(IRepository<Lesson,Guid> lessonRepository,
    IRepository<UserLessonProgress, Guid> userProgressRepository,
    IRepository<UserExerciseProgress, Guid> userExerciseProgressRepository,
    ITeacherService teacherService) : IUserLessonProgressService
{
    public async Task<ServiceResultT<IndexUserLessonProgressViewModel>> GetUserLessonProgress(Guid lessonId, string? userId)
    {
        Guid? userGuidId = Guid.TryParse(userId, out var parsedId)
            ? parsedId
            : null;
        
        var lesson = await lessonRepository
            .GetAllAttached()
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return ServiceResultT<IndexUserLessonProgressViewModel>.Fail("Invalid operation.");
        }
        
        if (userGuidId != null)
        {
            
            var hasUserProgress = await userProgressRepository
                .FirstorDefaultAsync(x => x.LessonId == lessonId && x.UserId == userGuidId);
          
            if (hasUserProgress == null)
            {
                return ServiceResultT<IndexUserLessonProgressViewModel>.Fail("Invalid operation.");
            }
            
            var model = new IndexUserLessonProgressViewModel()
            {
                IsUnlocked = hasUserProgress.IsUnlocked,
                IsCompleted = hasUserProgress.IsCompleted,
                CompletedAt = hasUserProgress.CompletedAt?.ToString("dd/MM/yyyy"),
            };
            
            return ServiceResultT<IndexUserLessonProgressViewModel>.Success(model);
        }
        
        return ServiceResultT<IndexUserLessonProgressViewModel>.Success(new IndexUserLessonProgressViewModel
        {
            IsUnlocked = false,
            IsCompleted = false,
            CompletedAt = null
        });
    }

    public async Task<ServiceResultT<int>> GetCourseProgressPercent(Guid courseId, Guid userId)
    {
        var totalLessons = await lessonRepository
            .GetAllAttached()
            .Where(l => l.CourseId == courseId)
            .CountAsync();

        if (totalLessons == 0)
        {
            return ServiceResultT<int>.Success(0);
        }
        
        var completedLessons = await userProgressRepository
            .GetAllAttached()
            .Where(up => up.IsCompleted && up.UserId == userId && up.Lesson.CourseId == courseId)
            .CountAsync();
        
        var percentInt = (int)Math.Round((completedLessons / (double)totalLessons) * 100);
        
        return ServiceResultT<int>.Success(percentInt);
    }

    public async Task<ServiceResultT<bool>> IsLessonUnlockedForAUserAsync (string lessonId ,string userId)
    {
        bool isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (isTeacher)
        {
            return ServiceResultT<bool>.Success(true);
        }
        
        Guid? userGuid = Guid.TryParse(userId, out Guid parsedUserId) ? parsedUserId : null;
        if (userGuid == null)
        {
            return ServiceResultT<bool>.Fail("Invalid operation.");
        }
        
        Guid? lessonGuid = Guid.TryParse(lessonId, out Guid parsedLessonId) ? parsedLessonId : null;
        if (lessonGuid == null)
        {
            return ServiceResultT<bool>.Fail("Invalid operation.");
        }
        
        bool hasProgress = await userProgressRepository
            .GetAllAttached()
            .AnyAsync(x => x.LessonId == lessonGuid && x.UserId == userGuid && x.IsUnlocked);
        
        return ServiceResultT<bool>.Success(hasProgress);
    }

    public async Task<bool> TryCompleteLessonProgressAsync(Guid lessonId, Guid userId)
    {
        var userProgress = await userProgressRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(x => x.LessonId == lessonId && x.UserId == userId);

        if (userProgress == null)
        {
            return false;
        }

        var hasAny = await userExerciseProgressRepository
            .GetAllAttached()
            .AnyAsync(u => u.UserId == userId && u.LessonId == lessonId);

        if (!hasAny)
        {
            return false;
        }

        var hasNotCompleted = await userExerciseProgressRepository
            .GetAllAttached()
            .Where(u => u.UserId == userId && u.LessonId == lessonId)
            .AnyAsync(up => up.IsCompleted == false);
        
        if (hasNotCompleted)
        {
            return false;
        }
        userProgress.IsCompleted = true;
        userProgress.CompletedAt = DateTime.UtcNow;
        await userProgressRepository.SaveChangesAsync();
        
        await UnlockNextLessonAsync(lessonId, userId);

        return true;
    }

    public async Task<bool> UnlockNextLessonAsync(Guid lessonId, Guid userId)
    {
        var currentLesson = await lessonRepository
            .GetAllAttached()
            .Where(l => l.Id == lessonId)
            .Select(l => new { l.Id, l.CourseId, l.OrderIndex })
            .FirstOrDefaultAsync();

        if (currentLesson == null || currentLesson.CourseId == null)
        {
            return false;
        }

        var nextLesson = await lessonRepository
            .GetAllAttached()
            .Where(l => l.CourseId == currentLesson.CourseId && l.OrderIndex > currentLesson.OrderIndex)
            .OrderBy(l => l.OrderIndex)
            .Select(l => new { l.Id })
            .FirstOrDefaultAsync();
       
        if (nextLesson == null)
        {
            return false; 
        }

        var nextProgress = await userProgressRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(x => x.LessonId == nextLesson.Id && x.UserId == userId);

        if (nextProgress == null)
        {
            return false;
        }

        if (nextProgress.IsUnlocked)
        {
            return true;
        }

        nextProgress.IsUnlocked = true;
        await userProgressRepository.SaveChangesAsync();
        
        return true;
    }
}
