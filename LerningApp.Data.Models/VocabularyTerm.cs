namespace LerningApp.Data.Models;

public class VocabularyTerm
{
  
    public Guid Id { get; set; }= Guid.NewGuid();
    
    public Guid VocabularyCardId { get; set; }
    public VocabularyCard VocabularyCard { get; set; } = null!;
    public string Text { get; set; } = null!;

    // "en" oder "de"
    public string Side { get; set; } = null!;
    public bool IsPrimary { get; set; }
}