namespace LerningApp.Common;

public static class EntityValidationConstants
{
    public static class Lesson
    {
        public const int NameMinLength = 5;
        public const int NameMaxLength = 100;
        
        public const int ContentMinLength = 50;
        public const int ContentMaxLength = 500;
        
        public const int TargetMinLength = 10;
        public const int TargetMaxLength = 200;
      
        public const int OrderIndexMin = 1;
        public const int OrderIndexMax = 1000;
        
        public const int GrammarMinLength = 50;
        public const int GrammarMaxLength = 500;
        
        public const int ExerciseMinLength = 50;
        public const int ExerciseMaxLength = 500;
        
    }
    
    public static class Course
    {
        public const int NameMinLength = 5;
        public const int NameMaxLength = 100;
        
        public const int DescriptionMinLength = 500;
        public const int DescriptionMaxLength = 1000;
    }
    public static class Level
    {
        public const int NameMinLength = 2;
        public const int NameMaxLength = 3;
        
        public const int DescriptionMinLength = 200;
        public const int DescriptionMaxLength = 1500;
        
    }
    public static class VocabularyTerm
    {
        public const int WordMaxLength = 63;
        
        public const int GermanWordMinLength = 2;
        public const int GermanWordMaxLength = 63;
        
        public const int EnglishWordMinLength = 1;
        public const int EnglishWordMaxLength = 45;
        
        public const int BulgarianWordMinLength = 1;
        public const int BulgarianWordMaxLength = 39;
        
        public const int SideMinLength = 2;
        public const int SideMaxLength = 5;
        
        public const int GenderdMinLength = 2;
        public const int GenderMaxLength = 3;
        
        public const int ExampleSentenceMinLenght = 8;
        public const int ExampleSentenceMaxLenght = 500;
        
    }

    public static class PartOfSpeech
    {
        public const int PartOfSpeechMinLength = 3;
        public const int PartOfSpeecheMaxLength = 30;
    }

    public static class LessonSection
    {
        public const int TypeMinLength = 5;
        public const int TypeMaxLength = 20;
        
        public const int ContentMinLength = 5;
        public const int ContentMaxLength = 7000;
    }

    public static class MultipleChoiceExercise
    {
        public const int QuestionMinLength = 10;
        public const int QuestionMaxLength = 250;
        
        public const int AnswerMinLength = 2;
        public const int AnswerMaxLength = 100;
    }
}