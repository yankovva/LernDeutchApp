using LerningApp.Web.ViewModels.PartOfSpeech;

namespace LerningApp.Services.Data.Interfaces;

public interface IPartOfSpeechService
{
  Task<IList<PartOfSpeechOptionsViewModel>> GetAllPartOfSpeechOptionsAsync();   
}