using PubQuizMediaServer.Models;
using System.Text.Json;

namespace PubQuizMediaServer.Util
{
    public class QuestionMediaPermissionCache
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuestionMediaPermissionCache> _logger;
        private readonly IConfiguration _configuration;

        private Dictionary<int, HashSet<int>> _permissions = new();

        public IReadOnlyDictionary<int, HashSet<int>> Permissions => _permissions;

        public QuestionMediaPermissionCache(HttpClient httpClient, IConfiguration configuration, ILogger<QuestionMediaPermissionCache> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Initialize()
        {
            await UpdatePermissions();
        }

        public async Task UpdatePermissions()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["BackendAddress"]}/permission");
                request.Headers.Add("X-API-KEY", _configuration["ApiKey"]);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<QuestionMediaPermissions>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result != null)
                    {
                        _permissions = result.Permissions;
                        _logger.LogInformation("Permissions updated successfully.");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize permissions.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Failed to get permissions: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception when updating permissions.");
            }
        }

        public void AddEditionPermission(EditionPermissionDto permissionDto)
        {
            lock (_permissions)
            {
                if (!_permissions.TryGetValue(permissionDto.EditionId, out var users))
                {
                    users = new HashSet<int>();
                    _permissions[permissionDto.EditionId] = users;
                }

                foreach(var userId in permissionDto.UserIds)
                {
                    if (!users.Contains(userId))
                    {
                        users.Add(userId);
                    }
                }
            }
        }

        public void DeleteEditionPermission(EditionPermissionDto permissionDto)
        {
            lock (_permissions)
            {
                if (!_permissions.TryGetValue(permissionDto.EditionId, out var users))
                {
                    return;
                }

                foreach (var userId in permissionDto.UserIds)
                {
                    users.Remove(userId);

                    if (users.Count == 0)
                        _permissions.Remove(permissionDto.EditionId);
                }
            }
        }

        public void AddUserPermission(UserPermissionDto permissionDto)
        {
            lock (_permissions)
            {
                foreach (var editionId in permissionDto.EditionIds)
                {
                    if (!_permissions.TryGetValue(editionId, out var userList))
                    {
                        userList = new HashSet<int>();
                        _permissions[editionId] = userList;
                    }

                    if (!userList.Contains(permissionDto.UserId))
                        userList.Add(permissionDto.UserId);
                }
            }
        }

        public void DeleteUserPermission(UserPermissionDto permissionDto)
        {
            lock (_permissions)
            {
                foreach (var editionId in permissionDto.EditionIds)
                {
                    if (_permissions.TryGetValue(editionId, out var users))
                    {
                        users.Remove(permissionDto.UserId);

                        if (users.Count == 0)
                            _permissions.Remove(editionId);
                    }
                }
            }
        }
    }
}
