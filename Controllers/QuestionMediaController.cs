using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubQuizMediaServer.Util;
using PubQuizMediaServer.Util.Extensions;
using PubQuizMediaServer.Util.MediaSanitizers;
using System.Security.Claims;

namespace PubQuizMediaServer.Controllers;

//[Authorize]
[ApiController]
[Route("question")]
public class QuestionMediaController : ControllerBase
{
    private readonly QuestionMediaPermissionCache _permissionCache;

    public QuestionMediaController(QuestionMediaPermissionCache permissionCache)
    {
        _permissionCache = permissionCache;
    }

    [HttpPost("{mediaType:regex(image|audio|video)}/{editionId}")]
    public async Task<IActionResult> UploadMedia(string mediaType, int editionId)
    {
        if (!User.IsBackend())
            return Unauthorized();

        var file = Request.Form.Files[mediaType];

        if (file == null)
            return BadRequest("No file provided under expected key.");

        string savedFileName;

        try
        {
            switch (mediaType)
            {
                case "image":
                    savedFileName = await ImageSanitazer.SaveQuestionImage(file, editionId);
                    break;
                case "audio":
                    savedFileName = await AudioSanitizer.SaveQuestionAudio(file, editionId);
                    break;
                case "video":
                    savedFileName = await VideoSanitizer.SaveQuestionVideo(file, editionId);
                    break;
                default:
                    return BadRequest("Unsupported media type.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Error saving media: {ex.Message}");
        }

        return Ok(savedFileName);
    }

    [HttpGet("{mediaType:regex(image|audio|video)}/{editionId}/{fileName}")]
    public IActionResult GetMedia(string mediaType, int editionId, string fileName)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("User ID not found or invalid.");
        }

        if (!_permissionCache.Permissions.TryGetValue(editionId, out var allowedUsers) || !allowedUsers.Contains(userId))
        {
            return Forbid("User does not have permission to access this media.");
        }

        string folderPath = mediaType switch
        {
            "image" => Path.Combine(MediaPaths.Private.QuestionImage, editionId.ToString()),
            "audio" => Path.Combine(MediaPaths.Private.QuestionAudio, editionId.ToString()),
            "video" => Path.Combine(MediaPaths.Private.QuestionVideo, editionId.ToString()),
            _ => null!
        };

        if (folderPath == null)
            return BadRequest("Unsupported media type.");

        var fullPath = Path.Combine(folderPath, fileName);
        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        var mime = GetMimeType(fullPath);
        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        if (mediaType == "audio")
            Console.WriteLine(stream.Length);
        return File(stream, mime);
    }

    private static string GetMimeType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".mp3" => "audio/mpeg",
            ".mp4" => "video/mp4",
            _ => "application/octet-stream"
        };
    }
}
