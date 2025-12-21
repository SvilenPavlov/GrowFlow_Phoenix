using AutoMapper;
using GrowFlow_Phoenix.Data;
using GrowFlow_Phoenix.DTOs.Leviathan.Employee;
using GrowFlow_Phoenix.Models;
using GrowFlow_Phoenix.Models.Leviathan;
using GrowFlow_Phoenix.Models.Phoenix;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GrowFlow_Phoenix.Infrastructure.Leviathan.Services
{
    public class LeviathanSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LeviathanSyncService> _logger;
        
        public LeviathanSyncService(ILogger<LeviathanSyncService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
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
                    await manager.RetryUnsuccesfullySyncedEmployees(stopToken);
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
                await Task.Delay(TimeSpan.FromMinutes(5), stopToken);
            }
        }

       
    }

}
