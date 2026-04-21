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
        Inactive = 5,
    }
    public enum ExerciseDifficultyLevel
    {
        VeryEasy = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        VeryHard = 4
    }
    public enum ServiceErrorType
    {
        Validation,
        NotFound,
        AccessDenied,
        Conflict,
        General
    }

    public enum CourseStatus
    {
        Draft = 1,
        Published = 2,
        Inactive = 3,
        Deleted = 4,
    }

}