namespace LerningApp.Common;

public static class EntityErrorMessages
{
   public static class Common
   {
      public const string AccessDeniedMessage = "You do not have permission to perform this action.";
   }
   public static class File
   {
      public const string InvalidFileMessage = "File is invalid or exceeds the maximum allowed size.";
      public const string MissingFileMessage = "Please select a valid file.";
   }
   public static class PartOfSpeech
   {
      public const string InvalidPartOfSpeechIdMessage = "Invalid part of speech ID.";
      public const string PartOfSpeechNotFoundMessage = "Part of speech not found.";
   }
   public static class Course
   {
      //Entity Validation Messages
      public const string CourseLevelValidationMessage = "Please enter a valid course level";
      public const string CourseNameValidationMessage = "Please enter a valid course name";
      public const string CourseDescriptionValidationMessage = "Please enter a valid course description";
      
      //Error Messages
      public const string InvalidCourseIdMessage = "Invalid course ID.";
      public const string CourseNotFoundMessage = "Course not found.";
      public const string AlreadyEnrolled = "You are already enrolled in this course.";
   }

   public static class Lesson
   {
      public const string InvalidLessonIdMessage = "Invalid lesson ID.";
      public const string LessonNotFoundMessage = "Lesson not found.";
   }
   public static class Card
   {
      public const string InvalidCardIdMessage = "Invalid vocabulary card ID.";
      public const string CardNotFoundMessage = "Vocabulary card not found.";
   }
   
   public static class Level
   {
      public const string InvalidLevelIdMessage = "Invalid level ID.";
      public const string LevelNotFoundMessage = "Level not found.";
   }
}