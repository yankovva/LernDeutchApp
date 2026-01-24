namespace LerningApp.Data.Models;

public class VocabularyTerm
{
    public VocabularyTerm()
    {
        this.Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    
    public Guid VocabularyItemId { get; set; }
    public VocabularyItem VocabularyItem { get; set; } = null!;
    public string Text { get; set; } = null!;

    // "en" oder "de"
    public string Side { get; set; } = null!;

    public bool IsPrimary { get; set; }
}