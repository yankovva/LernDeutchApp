namespace LerningApp.Data.Models;

public class MultipleChoiceQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string? Question { get; set; }
    
    public Guid PublisherId { get; set; } 
    
    public Teacher Publisher { get; set; } = null!;
    
    public Guid MultipleChoiceExerciseId { get; set; }
    
    public MultipleChoiceExercise MultipleChoiceExercise { get; set; }= null!;
    
    public ICollection<MultipleChoiceExerciseOption> Options { get; set; } = new List<MultipleChoiceExerciseOption>();
    
    public bool IsDeleted  { get; set; }
}