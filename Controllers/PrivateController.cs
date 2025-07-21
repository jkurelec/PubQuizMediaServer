using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubQuizMediaServer.Util;
using PubQuizMediaServer.Util.Extensions;

namespace PubQuizMediaServer.Controllers
{
    [Route("private")]
    [ApiController]
    public class PrivateController : ControllerBase
    {
        [HttpPost("update/organization")]
        public async Task<IActionResult> UpdateOrganization(IFormFile image)
        {
            if (!User.IsBackend())
                return Unauthorized();

            var newFileName = await ImageSanitazer.SaveProfileImageAsync(image, MediaPaths.Public.Organization);

            return Ok(newFileName);
        }

        [HttpPost("update/quiz")]
        public async Task<IActionResult> UpdateQuiz(IFormFile image)
        {
            if (!User.IsBackend())
                return Unauthorized();

            var newFileName = await ImageSanitazer.SaveProfileImageAsync(image, MediaPaths.Public.Quiz);

            return Ok(newFileName);
        }
    }
}
