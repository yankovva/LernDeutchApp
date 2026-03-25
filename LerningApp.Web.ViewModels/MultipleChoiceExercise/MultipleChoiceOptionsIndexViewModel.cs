namespace LerningApp.Web.ViewModels.MultipleChoiceExercise;

public class MultipleChoiceOptionsIndexViewModel
{
    public string Answer { get; set; } = null!;
    
    public bool IsCorrect { get; set; }
    
    public int OrderIndex { get; set; }
}