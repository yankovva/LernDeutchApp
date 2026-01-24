namespace LerningApp.Data.Models;

public class VocabularyItem
{
    public VocabularyItem()
    {
        this.Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public ICollection<VocabularyTerm> Terms { get; set; }
        = new List<VocabularyTerm>();
}