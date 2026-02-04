namespace LerningApp.Web.ViewModels.VocabularyCard;

public class VocabularyCardDetailsViewModel
{
    public string Id { get; set; } = null!;
    
    public string LessonId { get; set; } = null!;
    public string LessonName { get; set; } = null!;
    public string GermanWord { get; set; } = null!;
    public string? BulgarianTranslation { get; set; }
    public string? EnglishTranslation { get; set; }
    public string PartOfSpeech { get; set; } = null!;
    
    public List<string> BulgarianSynonyms { get; set; } = new();
    
    public List<string> EnglishSynonyms { get; set; } = new();
}