using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class LessonSection
{
    [Comment("PK Unique Identifier")]
    public Guid Id { get; set; }
    
    [Comment("Foreign key to Lesson")]
    public Guid LessonId { get; set; }
    
    public Lesson Lesson { get; set; }
    
    [Comment("The Type of the LessonSection - grammar/exercise")]
    public string Type { get; set; } = null!;
    
    [Comment("The Content of the LessonSection")]
    public string Content { get; set; } = null!;
    
    [Comment("The Order Index of the LessonSection")]
    public int OrderIndex { get; set; }
}