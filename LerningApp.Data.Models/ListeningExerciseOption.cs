using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class ListeningExerciseOption
{
    public Guid  Id { get; set; } = Guid.NewGuid();
    
    [Comment("The answer of the exercise")]
    public string Answer { get; set; } = null!;
    
    [Comment("Whether the answer is true or false")]
    public bool isCorrect { get; set; } 
    
    [Comment("Order index of the answer")]
    public int OrderIndex { get; set; }
    
    [Comment("Foreign key to ListeningExercise")]
    public Guid ListeningExerciseId { get; set; }
    
    [Comment("ListeningExercise Reference")]
    public ListeningExercise ListeningExercise { get; set; } = null!;
}