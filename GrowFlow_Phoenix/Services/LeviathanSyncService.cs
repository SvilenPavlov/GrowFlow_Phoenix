using GrowFlow_Phoenix.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowFlow_Phoenix.Services
{
    public class LeviathanSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LeviathanSyncService> _logger;

        public LeviathanSyncService(
            IServiceScopeFactory scopeFactory,
            ILogger<LeviathanSyncService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //This methods are commented out as they were blocking the swagger browser page.
            _logger.LogInformation("Leviathan sync service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DownloadSnapshot(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Leviathan snapshot download failed");
                }

                try
                {
                    await SynceEmployeesToSnapshot(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Local Leviathan snapshot sync failed");
                }

                // Wait 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task DownloadSnapshot(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var leviathanClient = scope.ServiceProvider.GetRequiredService<LeviathanClient>();
            var db = scope.ServiceProvider.GetRequiredService<PhoenixDbContext>();

            var leviathanEmployees = await leviathanClient.GetEmployeesAsync(); // did not include the cancellation token for simplicity 

            for (int i = 0; i < leviathanEmployees.Count(); i++)
            {
                var leviathanEmployee = leviathanEmployees[i];
                var localEntry = new LeviathanSnapshotEntry
                {
                    //Id = Guid.NewGuid(),
                    //Name = leviathanEmployee.Name,
                    //Address = leviathanEmployee.Address,
                    //LeviathanEmployeeId = leviathanEmployee.LeviathanEmployeeId,
                    //LeviathanId = leviathanEmployee.LeviathanId,
                };

                db.LeviathanSnapshotEntries.Add(localEntry);
            }

            await db.SaveChangesAsync(ct);
        }

        private async Task SynceEmployeesToSnapshot(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var leviathanClient = scope.ServiceProvider.GetRequiredService<LeviathanClient>();
            var db = scope.ServiceProvider.GetRequiredService<PhoenixDbContext>();

            var leviathanEmployees = await leviathanClient.GetEmployeesAsync(); // did not include the cancellation token for simplicity 
            var phoenixEmployees = await db.Employees.ToListAsync();

            try
            {
                for (int i = 0; i < leviathanEmployees.Count(); i++)
                {
                    var leviathanEmployee = leviathanEmployees[i];
                    // map + upsert
                    var phoenixEmployeeMatch = phoenixEmployees.FirstOrDefault(x => x.Id == leviathanEmployee.LeviathanEmployeeId);
                    if (phoenixEmployeeMatch != null)
                    {
                        var firstName = string.Empty;
                        var lastName = string.Empty;
                        if (string.IsNullOrEmpty(leviathanEmployee.Name) && string.IsNullOrWhiteSpace(leviathanEmployee.Name))
                        {
                            firstName = leviathanEmployee.Name.Split(' ')[0]; //basic name structure assumption
                            lastName = string.Join(" ", leviathanEmployee.Name.Split(' ').Skip(1)); //everything after first string considered last name
                        }

                        phoenixEmployeeMatch.FirstName = firstName;
                        phoenixEmployeeMatch.LastName = lastName;
                        phoenixEmployeeMatch.LeviathanId = leviathanEmployee.LeviathanId.ToString();
                        phoenixEmployeeMatch.LastSyncedAt = DateTime.UtcNow;
                        phoenixEmployeeMatch.IsSynced = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during mapping Leviathan employees to Phoenix employees");
            }

            await db.SaveChangesAsync(ct);

        }
    }

}
