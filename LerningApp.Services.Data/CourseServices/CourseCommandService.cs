using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Course;

using Microsoft.AspNetCore.Identity;

using static LerningApp.Common.EntityErrorMessages.Level;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.EntityErrorMessages.Course;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data.CourseServices;

public class CourseCommandService( IRepository<Course, Guid> courseRepository,
    IRepository<Level, Guid> levelRepository,
    ITeacherService teacherService,
    UserManager<ApplicationUser> userManager) : ICourseCommandService
{
    public async Task<ServiceResult> AddCourseAsync(AddCourseViewModel model, string userId)
    {
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        if (teacherId == null)
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        if (string.IsNullOrWhiteSpace(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail(InvalidLevelIdMessage);
        }

        Level? level = await levelRepository
            .GetByIdAsync(levelId);

        if (level == null)
        {
            return ServiceResult.Fail(LevelNotFoundMessage);
        }
        
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            LevelId = levelId,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            PublisherId = teacherId.Value,
            Price = model.Price
        };

        courseRepository.Add(course);
        await courseRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }
     public async Task<ServiceResultT<CourseEditViewModel>> GetCourseEditByIdAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResultT<CourseEditViewModel>.Fail(InvalidCourseIdMessage);
        }

        Course? course = await courseRepository
            .GetByIdAsync(courseId);

        if (course == null)
        {
            return ServiceResultT<CourseEditViewModel>.Fail(CourseNotFoundMessage);
        }
        
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || course.PublisherId != teacherId))
        {
            return ServiceResultT<CourseEditViewModel>.Fail(AccessDeniedMessage);
        }

        CourseEditViewModel model = new CourseEditViewModel()
        {
            Id = courseId.ToString(),
            Name = course.Name,
            Price = course.Price,
            Description = course.Description,
            LevelId = course.LevelId.ToString()
        };

        return ServiceResultT<CourseEditViewModel>.Success(model);
    }

    public async Task<ServiceResult> PostEditCourseAsync(CourseEditViewModel model, string id, string userId)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid courseId))
        {
            return ServiceResult.Fail(InvalidCourseIdMessage, nameof(id));
        }

        Course? courseToChange = await courseRepository
            .GetByIdAsync(courseId);

        if (courseToChange == null)
        {
            return ServiceResult.Fail(CourseNotFoundMessage, nameof(id));
        }
       
        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || courseToChange.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        if (string.IsNullOrEmpty(model.LevelId) || !Guid.TryParse(model.LevelId, out Guid levelId))
        {
            return ServiceResult.Fail(InvalidLevelIdMessage, nameof(model.LevelId));
        }

        var levelExists = await levelRepository
            .GetByIdAsync(levelId);

        if (levelExists == null)
        {
            return ServiceResult.Fail(LevelNotFoundMessage, nameof(model.LevelId));
        }

        courseToChange.Name = model.Name;
        courseToChange.Description = model.Description;
        courseToChange.LevelId = levelId;
        courseToChange.Price = model.Price;

        await courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}