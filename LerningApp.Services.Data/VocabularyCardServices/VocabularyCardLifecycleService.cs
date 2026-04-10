using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LerningApp.Common.EntityErrorMessages.Card;
using static LerningApp.Common.EntityErrorMessages.Common;
using static LerningApp.Common.ApplicationConstants;

namespace LerningApp.Services.Data.VocabularyCardServices;

public class VocabularyCardLifecycleService(
    IRepository<VocabularyCard, Guid> vocabularyCardRepository,
    ITeacherService teacherService,
    IFileService fileService,
    UserManager<ApplicationUser> userManager) : IVocabularyCardLifecycleService
{
    public async Task<ServiceResult> DeleteCardByIdAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResult.Fail(InvalidCardIdMessage);
        }

        var card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Lesson)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return ServiceResult.Fail(CardNotFoundMessage);
        }

        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || card.Lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }

        if (card.ImagePath != null)
        {
            fileService.DeleteFile(card.ImagePath);
        }

        vocabularyCardRepository.DeleteByEntity(card);
        await vocabularyCardRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SoftDeleteCardAsync(string id, string userId)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid cardId))
        {
            return ServiceResult.Fail(InvalidCardIdMessage);
        }

        VocabularyCard? card = await vocabularyCardRepository
            .GetAllAttached()
            .Include(c => c.Terms)
            .Include(c => c.Lesson)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return ServiceResult.Fail(CardNotFoundMessage);
        }

        Guid? teacherId = await teacherService.GetTeacherIdAsync(userId);
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user!, AdminRole);
        
        if (!isAdmin && (teacherId == null || card.Lesson.PublisherId != teacherId))
        {
            return ServiceResult.Fail(AccessDeniedMessage);
        }
        
        card.IsDeleted = true;

        foreach (var term in card.Terms)
        {
            term.IsDeleted = true;
        }

        await vocabularyCardRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
