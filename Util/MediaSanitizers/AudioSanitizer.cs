using PubQuizMediaServer.Exceptions;

namespace PubQuizMediaServer.Util.MediaSanitizers
{
    public static class AudioSanitizer
    {
        private const long MaxAudioSize = 20 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".mp3" };

        public static async Task<string> SaveQuestionAudio(IFormFile audio, int editionId)
        {
            if (audio == null || audio.Length == 0)
                throw new ArgumentException("Invalid audio file.");

            if (audio.Length > MaxAudioSize)
                throw new BadRequestException("Audio file size exceeds the 20MB limit.");

            var extension = Path.GetExtension(audio.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new BadRequestException("Unsupported audio format.");

            var editionFolder = Path.Combine(MediaPaths.Private.QuestionAudio, $"{editionId}");
            Directory.CreateDirectory(editionFolder);

            var baseFileName = Path.GetFileNameWithoutExtension(audio.FileName);
            var finalFileName = baseFileName + ".mp3";
            var filePath = Path.Combine(editionFolder, finalFileName);
            var backupPath = Path.Combine(editionFolder, "old" + finalFileName);

            if (File.Exists(filePath))
                File.Move(filePath, backupPath);

            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await audio.CopyToAsync(stream);

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
