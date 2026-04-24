namespace LerningApp.Web.ViewModels.Teacher.Lesson;

public class LessonManageViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Target { get; set; }
    public string? Content { get; set; }
    public string? CourseName { get; set; }
    public string CreatedAt { get; set; } = null!;
    public int OrderIndex { get; set; }
    public bool IsDeleted { get; set; }

    public int TotalWordsCount { get; set; }
    public int MultipleChoiceCount { get; set; }
    public int ListeningExercisesCount { get; set; }
    public int TranslationExercisesCount { get; set; }
    public int TotalExercisesCount { get; set; }
    public int CompletedUsersCount { get; set; }

    public IEnumerable<LessonManageWordViewModel> Words { get; set; }
        = new List<LessonManageWordViewModel>();

    public IEnumerable<LessonManageMultipleChoiceViewModel> MultipleChoiceExercises { get; set; }
        = new List<LessonManageMultipleChoiceViewModel>();

    public IEnumerable<LessonManageListeningViewModel> ListeningExercises { get; set; }
        = new List<LessonManageListeningViewModel>();

    public IEnumerable<LessonManageTranslationViewModel> TranslationExercises { get; set; }
        = new List<LessonManageTranslationViewModel>();
}