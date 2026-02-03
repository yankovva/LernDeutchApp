namespace LerningApp.Data.Models;

public class VocabularyItem
{
   
    public Guid Id { get; set; }= Guid.NewGuid();

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public Guid PartOfSpeechId { get; set; }
    public PartOfSpeech PartOfSpeech { get; set; } = null!;
     public virtual ICollection<VocabularyTerm> Terms { get; set; }
        = new HashSet<VocabularyTerm>();
}