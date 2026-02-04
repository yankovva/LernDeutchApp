namespace LerningApp.Web.ViewModels.VocabularyCard;

public class VocabularyCardsIndexViewModel
{
    public string LessonId { get; set; } = null!;
    
    public string LessonName { get; set; } = null!;
    
    public IList<VocabularyCardRowViewModel> Cards { get; set; } 
        = new List<VocabularyCardRowViewModel>();
}
