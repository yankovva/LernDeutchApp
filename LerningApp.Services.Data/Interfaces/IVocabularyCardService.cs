using LerningApp.Common;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.AspNetCore.Http;

namespace LerningApp.Services.Data.Interfaces;

public interface IVocabularyCardService
{
    Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId, string userId);
    Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id, string userId);
    Task<ServiceResultT<VocabularyCardCreateInputModel>> GetCreateVocabularyCardAsync(string lessonId,string userId);
    Task<ServiceResult> CreateVocabularyCardAsync(VocabularyCardCreateInputModel model, string userId);
    Task<ServiceResultT<VocabularyCardEditInputModel>> GetCardEditByIdAsync(string id, string userId);
    Task<ServiceResult> PostCardEditByIdAsync( VocabularyCardEditInputModel model,string id, string userId);
    Task<ServiceResult> DeleteCardByIdAsync(string id, string userId);
    Task<ServiceResult> SoftDeleteCardAsync(string id, string userId);
} 