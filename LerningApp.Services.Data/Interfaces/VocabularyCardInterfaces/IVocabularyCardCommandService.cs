using LerningApp.Common;
using LerningApp.Web.ViewModels.VocabularyCard;

namespace LerningApp.Services.Data.Interfaces;

public interface IVocabularyCardCommandService
{
    Task<ServiceResultT<VocabularyCardCreateInputModel>> GetCreateVocabularyCardAsync(string lessonId, string userId);
    Task<ServiceResult> CreateVocabularyCardAsync(VocabularyCardCreateInputModel model, string userId);
    Task<ServiceResultT<VocabularyCardEditInputModel>> GetCardEditByIdAsync(string id, string userId);
    Task<ServiceResult> PostCardEditByIdAsync(VocabularyCardEditInputModel model, string id, string userId);
}
