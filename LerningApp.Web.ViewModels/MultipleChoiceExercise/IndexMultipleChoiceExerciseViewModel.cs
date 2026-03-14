namespace LerningApp.Web.ViewModels.MultipleChoiceExercise;

public class IndexMultipleChoiceExerciseViewModel
{
    public string Id { get; set; } = null!;
    public string Question { get; set; } = null!;
    
    public string CorrectAnswer { get; set; } = null!;
   
    public string FirstWrongAnswer { get; set; } = null!;
   
    public string? SecondWrongAnswer { get; set; } 
  
    public string? ThirdWrongAnswer { get; set; }
}