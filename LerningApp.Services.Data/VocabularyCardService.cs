using LerningApp.Common;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.VocabularyCard;

namespace LerningApp.Services.Data;

public class VocabularyCardService(
    IVocabularyCardQueryService queryService,
    IVocabularyCardCommandService commandService,
    IVocabularyCardLifecycleService lifecycleService) : IVocabularyCardService
{
    public async Task<ServiceResultT<VocabularyCardsIndexViewModel>> IndexGetAllCardsForALessonAsync(string lessonId, string userId)
    {
        return await queryService.IndexGetAllCardsForALessonAsync(lessonId, userId);
    }

    public async Task<ServiceResultT<VocabularyCardDetailsViewModel>> GetDetailsForACardAsync(string id, string userId)
    {
        return await queryService.GetDetailsForACardAsync(id, userId);
    }

    public async Task<ServiceResultT<VocabularyCardCreateInputModel>> GetCreateVocabularyCardAsync(string lessonId, string userId)
    {
        return await commandService.GetCreateVocabularyCardAsync(lessonId, userId);
    }

    public async Task<ServiceResult> CreateVocabularyCardAsync(VocabularyCardCreateInputModel model, string userId)
    {
        return await commandService.CreateVocabularyCardAsync(model, userId);
    }

    public async Task<ServiceResultT<VocabularyCardEditInputModel>> GetCardEditByIdAsync(string id, string userId)
    {
        return await commandService.GetCardEditByIdAsync(id, userId);
    }

    public async Task<ServiceResult> PostCardEditByIdAsync(VocabularyCardEditInputModel model, string id,string userId)
    {
        return await commandService.PostCardEditByIdAsync(model, id, userId);
    }

    public async Task<ServiceResult> DeleteCardByIdAsync(string id,string userId)
    {
        return await lifecycleService.DeleteCardByIdAsync(id, userId);
    }

    public async Task<ServiceResult> SoftDeleteCardAsync(string id, string userId)
    {
        return await lifecycleService.SoftDeleteCardAsync(id, userId);
    }
}
