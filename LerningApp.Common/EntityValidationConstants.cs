namespace LerningApp.Common;

public static class EntityValidationConstants
{
    public static class Lesson
    {
        public const int NameMinLength = 5;
        public const int NameMaxLength = 100;
        
        public const int ContentMinLength = 300;
        public const int ContentMaxLength = 10000;
    }
    
    public static class Course
    {
        public const int NameMinLength = 5;
        public const int NameMaxLength = 100;
        
        public const int DescriptionMinLength = 100;
        public const int DescriptionMaxLength = 3000;
    }
}