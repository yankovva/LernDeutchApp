using LerningApp.Services.Data.Interfaces;
using static LerningApp.Common.EntityErrorMessages.File;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LerningApp.Services.Data;

public class FileService(IWebHostEnvironment environment) : IFileService
{
    public async Task<string> UploadFileAsync(IFormFile file, string folderName, string uniqueFileName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException(MissingFileMessage);

        string uploadFolder = Path.Combine(environment.WebRootPath, folderName);
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        string filePath = Path.Combine(uploadFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Path.Combine(folderName, uniqueFileName);
    }


    public void DeleteFile(string relativeFilePath)
    {
        if (string.IsNullOrWhiteSpace(relativeFilePath))
            return;

        string cleanPath = relativeFilePath.TrimStart('/', '\\');
        string fullPath = Path.Combine(environment.WebRootPath, cleanPath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public bool IsFileValid(IFormFile? file, string[] allowedExtensions, long maxSize)
    {
        if (file == null || file.Length == 0)
            return false;

        string fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
            return false;

        return file.Length <= maxSize;
    }
}