namespace LerningApp.Web.ViewModels.ListeningExercise;

public class IndexListeningQestionViewModel
{
    public string Question { get; set; } = null!;
    
    public List<IndexListeningOptionsViewModel> Options { get; set; } = new ();
}