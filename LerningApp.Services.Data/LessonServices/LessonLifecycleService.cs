using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Services.Data.Interfaces.LessonInterfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Lesson;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data.LessonServices;

public class LessonLifecycleService(IRepository<Lesson, Guid> lessonRepository,
    ITeacherService teacherService,
    UserManager<ApplicationUser> userManager) : ILessonLifecycleService
{
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
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        lesson.IsDeleted = true;
        await lessonRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
}