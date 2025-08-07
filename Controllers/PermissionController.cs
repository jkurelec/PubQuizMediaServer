using Microsoft.AspNetCore.Mvc;
using PubQuizMediaServer.Models;
using PubQuizMediaServer.Util;
using PubQuizMediaServer.Util.Extensions;

namespace PubQuizMediaServer.Controllers
{
    [Route("permissions")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly QuestionMediaPermissionCache _permissionCache;

        public PermissionController(QuestionMediaPermissionCache permissionCache)
        {
            _permissionCache = permissionCache;
        }

        [HttpPost("edition/add")]
        public void EditionAdd(EditionPermissionDto permission)
        {
            if (!User.IsBackend())
                return;

            _permissionCache.AddEditionPermission(permission);
        }

        [HttpPost("edition/remove")]
        public void EditionRemove(EditionPermissionDto permission)
        {
            if (!User.IsBackend())
                return;

            _permissionCache.DeleteEditionPermission(permission);
        }

        [HttpPost("user/add")]
        public void UserAdd(UserPermissionDto permission)
        {
            if (!User.IsBackend())
                return;

            _permissionCache.AddUserPermission(permission);
        }

        [HttpPost("user/remove")]
        public void UserRemove(UserPermissionDto permission)
        {
            if (!User.IsBackend())
                return;

            _permissionCache.DeleteUserPermission(permission);
        }
    }
}
