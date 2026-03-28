namespace LerningApp.Common;

public class Enums
{
    public enum TranslationLanguage
    {
        Bg = 1,
        En = 2
    }

    public enum ExerciseType
    {
        MultipleChoiceExercise = 1,
        TranslationExercise = 2,
        ListeningExercise = 3
    }

    public enum TeacherStatus
    {
        Draft = 1,
        Approved = 2,
        Rejected = 3,
        PendingReview = 4,
        Inactive = 5
    }
}