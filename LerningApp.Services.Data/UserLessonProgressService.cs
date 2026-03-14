using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.UserLessonProgress;
using Microsoft.EntityFrameworkCore;

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
}
