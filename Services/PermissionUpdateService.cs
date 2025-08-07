using PubQuizMediaServer.Util;

namespace PubQuizMediaServer.Services
{
    public class PermissionUpdateService : IHostedService, IDisposable
    {
        private readonly ILogger<PermissionUpdateService> _logger;
        private readonly QuestionMediaPermissionCache _permissionCache;
        private Timer? _timer;

        public PermissionUpdateService(QuestionMediaPermissionCache permissionCache, ILogger<PermissionUpdateService> logger)
        {
            _permissionCache = permissionCache;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Permission update service started.");

            await _permissionCache.UpdatePermissions();

            var now = DateTime.UtcNow;
            var next8Am = now.Date.AddHours(8);

            if (next8Am <= now)
                next8Am = next8Am.AddDays(1);

            var initialDelay = next8Am - now;

            _timer = new Timer(async _ =>
            {
                await _permissionCache.UpdatePermissions();
            }, null, initialDelay, TimeSpan.FromDays(1));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Permission update service stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() =>
            _timer?.Dispose();
    }

}
