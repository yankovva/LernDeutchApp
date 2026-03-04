using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data;

public class TeacherService(IRepository<Teacher, Guid> teacherRepository) : ITeacherService
{
    public async Task<Guid?> GetTeacherIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return null;
        }
        
        Teacher? teacher = await teacherRepository
            .FirstorDefaultAsync(t => t.UserId == userGuid);
        
        return teacher?.Id;
    }
}