using LerningApp.Common;
using LerningApp.Web.ViewModels.VocabularyCard;
using Microsoft.AspNetCore.Http;

namespace LerningApp.Services.Data.Interfaces;

public interface IVocabularyCardService
{
    Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId);
    Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id);
    Task<ServiceResult> CreateVocabularyCardAsync(VocabularyCardCreateInputModel model);
    Task<ServiceResultT<VocabularyCardEditInputModel>> GetCardEditByIdAsync(string id);

}