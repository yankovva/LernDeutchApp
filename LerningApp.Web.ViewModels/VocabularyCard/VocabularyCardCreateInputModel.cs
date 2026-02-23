using System.ComponentModel.DataAnnotations;
using LerningApp.Web.ViewModels.PartOfSpeech;
using Microsoft.AspNetCore.Http;
using static LerningApp.Common.EntityValidationConstants.VocabularyTerm;
namespace LerningApp.Web.ViewModels.VocabularyCard;

public class VocabularyCardCreateInputModel
{
    [Required]
    public string LessonId { get; set; } = null!;
    [Required]
    [MaxLength(GermanWordMaxLength)]
    [MinLength(GermanWordMinLength)]
    public string GermanWord { get; set; } = null!;
    
    [MaxLength(EnglishWordMaxLength)]
    [MinLength(EnglishWordMinLength)]
    [Required]
    public string EnglishWord { get; set; } = null!;
    
    [MaxLength(BulgarianWordMaxLength)]
    [MinLength(BulgarianWordMinLength)]
    [Required]
    public string BulgarianWord { get; set; } = null!;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; } 
    
    public IFormFile Image { get; set; } = null!;
    
    [MinLength(GenderdMinLength)]
    [MaxLength(GenderMaxLength)]
    //der die das
    public string? Gender { get; set; }
    
    [MaxLength(ExampleSentenceMaxLenght)]
    [MinLength(ExampleSentenceMinLenght)]
    [Required]
    public string ExampleSentence { get; set; } = null!;

    public bool IsPrimary { get; set; }
    
    [Required]
    public string PartOfSpeechId { get; set; } = null!;

    public IList<PartOfSpeechOptionsViewModel> PartOfSpeechOptions { get; set; } 
        = new List<PartOfSpeechOptionsViewModel>();
}