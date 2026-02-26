using LerningApp.Web.ViewModels.LessonSection;
using LerningApp.Web.ViewModels.MultipleChoiceExercise;
using LerningApp.Web.ViewModels.TranslationExercise;

namespace LerningApp.Web.ViewModels.Lesson;

public class LessonContentViewModel
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public string Content { get; set; } = null!;
    
    public int OrderIndex { get; set; }
    
    public string? CourseId { get; set; }
    
    public string? CourseName { get; set; }

    public int WordCount { get; set; }
    
    public string Target { get; set; } = null!;
    public string PublisherId { get; set; } = null!;

    public IList<IndexMultipleChoiceExerciseViewModel> MultipleChoiceExercises { get; set; }
        = new List<IndexMultipleChoiceExerciseViewModel>();
    
    public IList<IndexTranslationExerciseViewModel> TranslationExercises { get; set; }
        = new List<IndexTranslationExerciseViewModel>();
    
    public  IList<LessonSectionViewModel> LessonSections { get; set; } 
        = new List<LessonSectionViewModel>();
}