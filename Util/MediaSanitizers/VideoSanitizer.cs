using PubQuizMediaServer.Exceptions;

namespace PubQuizMediaServer.Util.MediaSanitizers
{
    public static class VideoSanitizer
    {
        private const long MaxVideoSize = 100 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".mp4" };

        public static async Task<string> SaveQuestionVideo(IFormFile video, int editionId)
        {
            if (video == null || video.Length == 0)
                throw new ArgumentException("Invalid video file.");

            if (video.Length > MaxVideoSize)
                throw new BadRequestException("Video file size exceeds the 100MB limit.");

            var extension = Path.GetExtension(video.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new BadRequestException("Unsupported video format.");

            var editionFolder = Path.Combine(MediaPaths.Private.QuestionVideo, $"{editionId}");
            Directory.CreateDirectory(editionFolder);

            var baseFileName = Path.GetFileNameWithoutExtension(video.FileName);
            var finalFileName = baseFileName + ".mp4";
            var filePath = Path.Combine(editionFolder, finalFileName);
            var backupPath = Path.Combine(editionFolder, "old" + finalFileName);

            if (File.Exists(filePath))
                File.Move(filePath, backupPath);

            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await video.CopyToAsync(stream);

                if (File.Exists(backupPath))
                    File.Delete(backupPath);
            }
            catch
            {
                if (File.Exists(backupPath))
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    File.Move(backupPath, filePath);
                }

                throw;
            }

            return finalFileName;
        }
    }
}
