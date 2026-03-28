using LerningApp.Web.ViewModels.Admin.Teacher;

namespace LerningApp.Services.Data.Interfaces.AdminInterfaces;

public interface IAdminTeacherService
{
    Task<IEnumerable<AdminTeacherIndexViewModel>> GetAllTeachersAsync();
}