using LerningApp.Web.ViewModels.UserProfile;

namespace LerningApp.Services.Data.Interfaces;

public interface IProfileService
{
    Task<UserProfileOverviewViewModel> IndexGetUserProfileOverviewModelAsync(Guid userId);
}