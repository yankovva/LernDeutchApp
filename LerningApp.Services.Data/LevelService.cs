using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Level;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data;

public class LevelService(IRepository<Level, Guid>levelRepository) : ILevelService
{
    public async Task<List<LevelOptionsViewModel>> GetAllLevelOptionsAsync()
    {
        var levels = await levelRepository
            .GetAllAttached()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new LevelOptionsViewModel
            {
                Id = c.Id.ToString(), 
                Name = c.Name
            })
            .ToListAsync();
        
        return levels;
    }
}