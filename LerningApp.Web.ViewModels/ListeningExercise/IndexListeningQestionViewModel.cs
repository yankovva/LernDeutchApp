namespace LerningApp.Web.ViewModels.ListeningExercise;

public class IndexListeningQestionViewModel
{
    public string Id { get; set; }= null!;
    public string Question { get; set; } = null!;
    
    public List<IndexListeningOptionsViewModel> Options { get; set; } = new ();
}