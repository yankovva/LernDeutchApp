using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class VocabularyTerm
{
    [Comment("PK Unique Identifier")]
    public Guid Id { get; set; }= Guid.NewGuid();
    
    [Comment("The Word of the VocabularyTerm")]
    public string Word { get; set; } = null!;
    
    [Comment("German gender (der/die/das), only for Side = 'de'")]
    public string? Gender { get; set; }
    
    [Comment("Example Sentence for the VocabularyTerm in German")]
    public string? ExampleSentence { get; set; }
    
    [Comment("Foreign key to VocabularyCard")]
    public Guid VocabularyCardId { get; set; }
    
    [Comment("VocabularyCard Reference")]
    public VocabularyCard VocabularyCard { get; set; } = null!;

    // "en" oder "de"
    [Comment("The Side of the VocabularyTerm(en/de)")]
    public string Side { get; set; } = null!;
    
    [Comment("Is the VocabularyTerm primary for the VocabularyCard")]
    public bool IsPrimary { get; set; }
}