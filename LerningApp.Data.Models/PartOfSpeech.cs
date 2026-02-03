namespace LerningApp.Data.Models;

public class PartOfSpeech
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;
    
    public virtual ICollection<VocabularyItem> VocabularyItems { get; set; } 
        = new HashSet<VocabularyItem>();
}