using LerningApp.Common;
using LerningApp.Data.Models;

namespace LerningApp.Services.Data.Interfaces;

public interface ITeacherService 
{
    Task<bool> IsUserTeacherAsync(string userId);
    Task<bool> HasUserTeacherEntityAsync(string userId);
    Task<Guid?> GetTeacherIdAsync(string userId);
}