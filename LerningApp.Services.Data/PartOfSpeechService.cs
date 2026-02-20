using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.PartOfSpeech;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data;

public class PartOfSpeechService(IRepository<PartOfSpeech, Guid> partOfSpeechRepository) : IPartOfSpeechService
{
    public async Task<IList<PartOfSpeechOptionsViewModel>> GetAllPartOfSpeechOptionsAsync()
    {
        var options = await partOfSpeechRepository
            .GetAllAttached()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(ps => new PartOfSpeechOptionsViewModel
            {
                Id = ps.Id.ToString(),
                Name = ps.Name,
            }).ToListAsync();
        
        return options;
    }
}