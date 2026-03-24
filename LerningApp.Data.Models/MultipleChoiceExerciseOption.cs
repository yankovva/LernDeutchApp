using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class MultipleChoiceExerciseOption
{
    public Guid  Id { get; set; } = Guid.NewGuid();
    
    [Comment("The answer of the exercise")]
    public string Answer { get; set; } = null!;
    
    [Comment("Whether the answer is correct or not")]
    public bool IsCorrect { get; set; } 
    
    [Comment("Order index of the answer")]
    public int OrderIndex { get; set; }
    
    [Comment("Foreign key to the MultipleChoise Question")]
    public Guid MultipleChoiceQuestionId { get; set; }
    
    [Comment("ListeningQuestion Reference")]
    public MultipleChoiceQuestion MultipleChoiceQuestion { get; set; } = null!;
}