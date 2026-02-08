using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class PartOfSpeech
{
    //TODO: Add short explanation of the part of the speech
    [Comment("PK Unique Identifier")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Comment("The Name of the PartOfSpeech")]
    public string Name { get; set; } = null!;
    
    [Comment("VocabularyCards with this PartOfSpeech")]
    public virtual ICollection<VocabularyCard> VocabularyCards { get; set; } 
        = new HashSet<VocabularyCard>();
}