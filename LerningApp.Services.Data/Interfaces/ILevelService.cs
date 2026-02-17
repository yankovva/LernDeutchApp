using LerningApp.Web.ViewModels.Level;

namespace LerningApp.Services.Data.Interfaces;

public interface ILevelService
{
    Task<List<LevelOptionsViewModel>> GetAllLevelOptionsAsync();
}