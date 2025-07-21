using PubQuizMediaServer.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace PubQuizMediaServer.Util
{
    public static class ImageSanitazer
    {
        private const long MaxImageSize = 10 * 1024 * 1024;

        public static async Task<string> SaveProfileImageAsync(IFormFile image, string folderPath, string fileName)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file.");

            if (image.Length > MaxImageSize)
                throw new BadRequestException("Image file size exceeds the 10MB limit.");

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowedExtensions.Contains(extension))
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

            if (string.IsNullOrWhiteSpace(fileName))
                throw new BadRequestException("File name is empty!");

            var filePath = Path.Combine(folderPath, fileName);
            var backupFilePath = Path.Combine(folderPath, "old" + fileName);

            if (File.Exists(filePath))
            {
                File.Move(filePath, backupFilePath);
            }

            try
            {
                await parsedImage.SaveAsJpegAsync(filePath, new JpegEncoder
                {
                    Quality = 85
                });

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

            return fileName;
        }
    }
}
