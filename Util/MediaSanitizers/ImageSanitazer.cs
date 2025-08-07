using PubQuizMediaServer.Exceptions;
using PubQuizMediaServer.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

public static class ImageSanitazer
{
    private const long MaxImageSize = 10 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public static async Task<string> SaveProfileImage(IFormFile image, string folderPath)
    {
        if (image == null || image.Length == 0)
            throw new ArgumentException("Invalid image file.");

        if (image.Length > MaxImageSize)
            throw new BadRequestException("Image file size exceeds the 10MB limit.");

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new BadRequestException("Unsupported file type.");

        Image parsedImage;
        try
        {
            parsedImage = await Image.LoadAsync(image.OpenReadStream());
        }
        catch (Exception)
        {
            throw new ForbiddenException();
        }

        parsedImage.Metadata.ExifProfile = null;

        Directory.CreateDirectory(folderPath);

        if (string.IsNullOrWhiteSpace(image.FileName))
            throw new BadRequestException("File name is empty!");

        var baseFileName = Path.GetFileNameWithoutExtension(image.FileName);
        var finalFileName = baseFileName + ".jpg";

        var filePath = Path.Combine(folderPath, finalFileName);
        var backupFilePath = Path.Combine(folderPath, "old" + finalFileName);

        if (File.Exists(filePath))
            File.Move(filePath, backupFilePath);

        try
        {
            await parsedImage.SaveAsJpegAsync(filePath, new JpegEncoder { Quality = 85 });

            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);
        }
        catch
        {
            if (File.Exists(backupFilePath))
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                File.Move(backupFilePath, filePath);
            }

            throw;
        }

        return finalFileName;
    }

    public static async Task<string> SaveQuestionImage(IFormFile image, int editionId)
    {
        if (image == null || image.Length == 0)
            throw new ArgumentException("Invalid image file.");

        if (image.Length > MaxImageSize)
            throw new BadRequestException("Image file size exceeds the 10MB limit.");

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new BadRequestException("Unsupported file type.");

        Image parsedImage;

        try
        {
            parsedImage = await Image.LoadAsync(image.OpenReadStream());
        }
        catch (Exception)
        {
            throw new ForbiddenException();
        }

        parsedImage.Metadata.ExifProfile = null;

        var editionFolder = Path.Combine(MediaPaths.Private.QuestionImage, $"{editionId}");
        Directory.CreateDirectory(editionFolder);

        if (string.IsNullOrWhiteSpace(image.FileName))
            throw new BadRequestException("File name is empty!");

        var baseFileName = Path.GetFileNameWithoutExtension(image.FileName);
        var finalFileName = baseFileName + ".jpg";

        var filePath = Path.Combine(editionFolder, finalFileName);
        var backupFilePath = Path.Combine(editionFolder, "old" + finalFileName);

        if (File.Exists(filePath))
            File.Move(filePath, backupFilePath);

        try
        {
            await parsedImage.SaveAsJpegAsync(filePath, new JpegEncoder { Quality = 85 });

            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);
        }
        catch
        {
            if (File.Exists(backupFilePath))
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                File.Move(backupFilePath, filePath);
            }

            throw;
        }

        return finalFileName;
    }
}
