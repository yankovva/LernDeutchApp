namespace LerningApp.Common;

public class ApplicationConstants
{
    public const string DefaultCardImagePath = "/images/VocabularyCardsImages/defaultcardimage.png";
    public const string DefaultCardDirectoryPath = "images/VocabularyCardsImages";
    public const string DefaultaListeningExerciseAudiosPath = "audios/ListeningExerciseAudios";

    public static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];
    public const long MaxFileSize = 5 * 1024 * 1024;
}