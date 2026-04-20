using LerningApp.Common;
using LerningApp.Web.ViewModels.Teacher;
using LerningApp.Web.ViewModels.UserProfile;

namespace LerningApp.Services.Data.Interfaces;

public interface IProfileService
{
    Task<UserProfileOverviewViewModel> IndexGetUserProfileOverviewModelAsync(Guid userId);
    Task<ServiceResultT<ProfileIndexViewModel>> GetTeacherProfileIndexViewModelAsync(Guid userId);
    Task<ServiceResultT<ProfileEditViewModel>> GetTeacherProfileEditViewModelAsync(Guid userId);
    
    Task<ServiceResult> PostTeacherProfileEditAsync(Guid userId, ProfileEditViewModel model);
}