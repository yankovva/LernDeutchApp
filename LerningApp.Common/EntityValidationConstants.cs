namespace LerningApp.Common;

public static class EntityValidationConstants
{
    public static class Lesson
    {
        public const int NameMinLength = 5;
        public const int NameMaxLength = 100;
        
        public const int ContentMinLength = 300;
        public const int ContentMaxLength = 10000;
        
        public const int OrderIndexMin = 1;
        public const int OrderIndexMax = 1000;
    }
    
    public static class Course
    {
        public const int NameMinLength = 5;
        public const int NameMaxLength = 100;
        
        public const int DescriptionMinLength = 500;
        public const int DescriptionMaxLength = 3000;
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
        public const int NameMinLength = 1;
        public const int NameMaxLength = 150;
        
        public const int SideMinLength = 2;
        public const int SideMaxLength = 5;
        
    }

    public static class PartOfSpeech
    {
        public const int PartOfSpeechMinLength = 3;
        public const int PartOfSpeecheMaxLength = 30;    }
}