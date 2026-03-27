using LerningApp.Common;

namespace LerningApp.Services.Data.Interfaces;

public interface IVocabularyCardLifecycleService
{
    Task<ServiceResult> DeleteCardByIdAsync(string id, string userId);
    Task<ServiceResult> SoftDeleteCardAsync(string id, string userId);
}
