namespace GrowFlow_Phoenix.Infrastructure.Leviathan.Services
{
    public class LeviathanSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LeviathanSyncService> _logger;
        private readonly int _delayMinutes;

        public LeviathanSyncService(ILogger<LeviathanSyncService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _delayMinutes = configuration.GetValue<int>("LeviathanApi:Settings:SyncDelay");
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            _logger.LogInformation("Leviathan sync service started.");
            using var scope = _scopeFactory.CreateScope();

            var manager = scope.ServiceProvider.GetRequiredService<LeviathanSyncManager>();
            while (!stopToken.IsCancellationRequested)
            {
                try
                {
                    //await manager.RetryUnsuccesfullySyncedEmployees(stopToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Leviathan sync retry failed");
                }

                try
                {
                    await manager.DownloadSnapshot(stopToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Leviathan snapshot download failed");
                }

                try
                {
                    await manager.SynceEmployeesFromSnapshot(stopToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Local Leviathan snapshot sync failed");
                }

                // Wait 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(_delayMinutes), stopToken);
            }
        }

       
    }

}
