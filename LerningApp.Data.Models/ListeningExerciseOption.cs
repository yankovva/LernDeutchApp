using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class ListeningExerciseOption
{
    public Guid  Id { get; set; } = Guid.NewGuid();
    
    [Comment("The answer of the exercise")]
    public string Answer { get; set; } = null!;
    
    [Comment("Whether the answer is correct or not")]
    public bool isCorrect { get; set; } 
    
    [Comment("Order index of the answer")]
    public int OrderIndex { get; set; }
    
    [Comment("Foreign key to the Listening Question")]
    public Guid ListeningQuestionId { get; set; }
    
    [Comment("ListeningQuestion Reference")]
    public ListeningQuestion ListeningQuestion { get; set; } = null!;
}