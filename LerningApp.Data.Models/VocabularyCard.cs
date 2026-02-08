using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class VocabularyCard
{
    [Comment("PK Unique Identifier")]
    public Guid Id { get; set; }= Guid.NewGuid();
    
    [Comment("Foreign key to Lesson")]
    public Guid LessonId { get; set; }
    
    [Comment("Lesson Reference")]
    public Lesson Lesson { get; set; } = null!;
    
    [Comment("Foreign key to PartOfSpeech")]
    public Guid PartOfSpeechId { get; set; }
    
    [Comment("PartOfSpeech Reference")]
    public PartOfSpeech PartOfSpeech { get; set; } = null!;
    
    [Comment("VocabularyTerm in this VocabularyCard")]
     public virtual ICollection<VocabularyTerm> Terms { get; set; }
        = new HashSet<VocabularyTerm>();
}