using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Web.ViewModels.UsersCouses;

namespace LerningApp.Services.Data.Interfaces;

public interface IUsersCoursesService
{
    Task<ServiceResultT<UsersCoursesIndexViewModel>> IndexGetAllUsersCoursesAsync(string userId);
}