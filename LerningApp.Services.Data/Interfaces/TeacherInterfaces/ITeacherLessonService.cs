using LerningApp.Web.ViewModels.Teacher;

namespace LerningApp.Services.Data.Interfaces.TeacherInterfaces;

public interface ITeacherLessonService
{
    Task<IEnumerable<LessonTeacherIndexViewModel>> GetAllLessonsAsync();
}