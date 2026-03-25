namespace LerningApp.Web.ViewModels.MultipleChoiceExercise;

public class IndexMultipleChoiceExerciseViewModel
{
    public string Id { get; set; } = null!;
    public string Question { get; set; } = null!;

    public List<MultipleChoiceOptionsIndexViewModel> Options { get; set; } =
        new List<MultipleChoiceOptionsIndexViewModel>();
}