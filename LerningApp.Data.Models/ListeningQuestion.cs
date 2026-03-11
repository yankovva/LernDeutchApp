namespace LerningApp.Data.Models;

public class ListeningQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Question { get; set; } = null!;
    
    public Guid PublisherId { get; set; } 
    
    public ApplicationUser Publisher { get; set; } = null!;
    
    public int DifficultyLevel { get; set; }
    
    public Guid ListeningExerciseId { get; set; }
    public ListeningExercise ListeningExercise { get; set; }= null!;
    public ICollection<ListeningExerciseOption> Options { get; set; } = new List<ListeningExerciseOption>();
    public bool IsDeleted  { get; set; }
}