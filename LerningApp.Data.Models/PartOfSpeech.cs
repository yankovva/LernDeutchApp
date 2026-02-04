namespace LerningApp.Data.Models;

public class PartOfSpeech
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;
    
    public virtual ICollection<VocabularyCard> VocabularyCards { get; set; } 
        = new HashSet<VocabularyCard>();
}