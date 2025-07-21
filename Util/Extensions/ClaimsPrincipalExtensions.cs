using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PubQuizMediaServer.Util.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsBackend(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return false;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var username = user.FindFirstValue(ClaimTypes.Name);

            var role = user.FindFirstValue(ClaimTypes.Role);

            const string expectedUserId = "0";
            const string expectedUsername = "Backend";
            const string expectedRole = "Admin";

            return userId == expectedUserId
                   && username == expectedUsername
                   && role == expectedRole;
        }
    }
}
