using LerningApp.Common;
using LerningApp.Web.ViewModels.VocabularyCard;

namespace LerningApp.Services.Data.Interfaces;

public interface IVocabularyCardService
{
    Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId);
    Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id);
}