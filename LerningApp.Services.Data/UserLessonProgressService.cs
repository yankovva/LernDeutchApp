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
    IRepository<UserCourse, object> userCourseRepository) : IUserLessonProgressService
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
}
