namespace LerningApp.Web.ViewModels.UsersCouses;

public class MyCourseCardViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string LevelName { get; set; } = null!;
    public DateTime StartedAt { get; set; } 
    public DateTime? CompletedAt { get; set; }
}