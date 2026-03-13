namespace LerningApp.Web.ViewModels.ListeningExercise;

public class IndexListeningExerciseViewModel
{
    public string Id { get; set; } = null!;
    
    public string AudioPath { get; set; } = null!;
    
    public List<IndexListeningQestionViewModel> Qestions { get; set; } = new ()!;
}