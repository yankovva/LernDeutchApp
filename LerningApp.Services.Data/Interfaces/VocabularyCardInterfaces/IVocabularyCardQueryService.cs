using LerningApp.Common;
using LerningApp.Web.ViewModels.VocabularyCard;

namespace LerningApp.Services.Data.Interfaces;

public interface IVocabularyCardQueryService
{
    Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId, string userId);
    Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id, string userId);
}
