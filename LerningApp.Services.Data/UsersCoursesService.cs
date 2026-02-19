using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.UsersCouses;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data;

public class UsersCoursesService(IRepository<UserCourse, object> userCourseRepository) : IUsersCoursesService
{
    public async Task<ServiceResultT<UsersCoursesIndexViewModel>> IndexGetAllUsersCoursesAsync(string userId)
    {
        List<MyCourseCardViewModel> courses =  await userCourseRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(uc => uc.Course)
            .ThenInclude(c => c.Level)
            .Where(uc => uc.UserId.ToString() == userId)
            .Select(uc => new MyCourseCardViewModel
            {
                Id = uc.Course.Id.ToString(),
                Name = uc.Course.Name,
                Description = uc.Course.Description,
                LevelName = uc.Course.Level.Name,
                StartedAt = uc.StartedAt,
                CompletedAt = uc.CompletedAt
            })
            .ToListAsync();
         
        var model = new UsersCoursesIndexViewModel { Courses = courses };
        
        return ServiceResultT<UsersCoursesIndexViewModel>.Success(model);
    }
}