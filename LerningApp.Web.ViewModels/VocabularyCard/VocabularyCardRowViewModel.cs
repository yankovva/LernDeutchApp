namespace LerningApp.Web.ViewModels.VocabularyCard;

public class VocabularyCardRowViewModel
{
    public string Id { get; set; } = null!;
    
    public string German { get; set; } = null!;
    
    public string? Bulgarian { get; set; }
    
    public string? English { get; set; }
    
    public string PartOfSpeech { get; set; } = null!;
}