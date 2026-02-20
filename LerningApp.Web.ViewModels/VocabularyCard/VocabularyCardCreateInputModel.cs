using System.ComponentModel.DataAnnotations;
using LerningApp.Web.ViewModels.PartOfSpeech;

namespace LerningApp.Web.ViewModels.VocabularyCard;

public class VocabularyCardCreateInputModel
{
    [Required]
    public string LessonId { get; set; } = null!;
    [Required]
    [MaxLength(63)]
    [MinLength(2)]
    public string GermanWord { get; set; } = null!;
    
    [MaxLength(45)]
    [MinLength(1)]
    [Required]
    public string EnglishWord { get; set; } = null!;
    
    [MaxLength(39)]
    [MinLength(1)]
    [Required]
    public string BulgarianWord { get; set; } = null!;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; } 
    
    [MaxLength(3)]
    //der die das
    public string? Gender { get; set; }
    
    [MaxLength(500)]
    public string? ExampleSentence { get; set; }

    public bool IsPrimary { get; set; }
    
    [Required]
    public string PartOfSpeechId { get; set; } = null!;

    public IList<PartOfSpeechOptionsViewModel> PartOfSpeechOptions { get; set; } 
        = new List<PartOfSpeechOptionsViewModel>();

}