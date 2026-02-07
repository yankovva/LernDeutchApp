namespace LerningApp.Web.ViewModels.UsersCouses;

public class UsersCoursesIndexViewModel
{
    public IEnumerable<MyCourseCardViewModel> Courses { get; set; } = new List<MyCourseCardViewModel>();
}