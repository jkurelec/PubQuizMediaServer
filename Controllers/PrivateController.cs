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
        public async Task<IActionResult> UpdateOrganization(IFormFile image, string fileName)
        {
            if (!User.IsBackend())
                return Unauthorized();

            var newFileName = await ImageSanitazer.SaveProfileImageAsync(image, MediaPaths.Public.Organization, fileName);

            return Ok(newFileName);
        }
    }
}
