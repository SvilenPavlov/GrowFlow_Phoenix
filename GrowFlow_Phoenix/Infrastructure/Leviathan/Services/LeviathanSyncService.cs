using AutoMapper;
using GrowFlow_Phoenix.Data;
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
        private readonly IMapper _mapper;
        public readonly string _employeeEndpoint;
        private readonly string _leviathanProviderName;
        public LeviathanSyncService(IServiceScopeFactory scopeFactory, ILogger<LeviathanSyncService> logger, IConfiguration configuration, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _mapper = mapper;
            _leviathanProviderName = configuration.GetValue<string>("LeviathanApi:ProviderName");
            _employeeEndpoint = configuration.GetValue<string>("LeviathanApi:Endpoints:EmployeeEndpoint");
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            //This methods are commented out as they were blocking the swagger browser page.
            _logger.LogInformation("Leviathan sync service started.");

            while (!stopToken.IsCancellationRequested)
            {
                try
                {
                    //maybe a retry unsuccesfullsyncs first?
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Leviathan sync retry failed");
                }

                try
                {
                    await DownloadSnapshot(stopToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Leviathan snapshot download failed");
                }

                try
                {
                    await SynceEmployeesFromSnapshot(stopToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Local Leviathan snapshot sync failed");
                }

                // Wait 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stopToken);
            }
        }

        private async Task DownloadSnapshot(CancellationToken stopToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var leviathanClient = scope.ServiceProvider.GetRequiredService<LeviathanClient>();
            var db = scope.ServiceProvider.GetRequiredService<PhoenixDbContext>();
            try
            {
                var leviathanEmployees = await leviathanClient.GetEmployeesAsync(stopToken); // did not include the cancellation token for simplicity 
                for (int i = 0; i < leviathanEmployees.Count(); i++)
                {
                    var localEntry = _mapper.Map<LeviathanEmployeeCache>(leviathanEmployees[i]);
                    localEntry.Id = Guid.NewGuid();
                    db.LeviathanSnapshotEntries.Add(localEntry);
                }
                try
                {
                    await db.SaveChangesAsync(stopToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed saving Leviathan snapshot entries");
                    throw;
                }

                await db.SaveChangesAsync(stopToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Leviathan does not respond to GET employees request");
            }
        }

        private async Task SynceEmployeesFromSnapshot(CancellationToken stopToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var leviathanClient = scope.ServiceProvider.GetRequiredService<LeviathanClient>();
            var db = scope.ServiceProvider.GetRequiredService<PhoenixDbContext>();

            var leviathanEmployees = await leviathanClient.GetEmployeesAsync(stopToken);
            var phoenixEmployees = await db.Employees.ToListAsync();

            try
            {
                for (int i = 0; i < leviathanEmployees.Count(); i++)
                {
                    var leviathanEmployee = leviathanEmployees[i];
                    var employeExternalIdEntry = db.EmployeeExternalIds.FirstOrDefault(x => x.Provider == _leviathanProviderName && x.ExternalId == leviathanEmployee.LeviathanId.ToString());
                    if (employeExternalIdEntry != null)
                    {
                        var phoenixEmployeeMatch = db.Employees.FirstOrDefault(x => x.Id == employeExternalIdEntry.EmployeeId);
                        _mapper.Map(leviathanEmployee, phoenixEmployeeMatch);
                        db.Update(phoenixEmployeeMatch!);
                    }

                }
                await db.SaveChangesAsync(stopToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during mapping Leviathan employees to Phoenix employees");
            }
        }
    }

}
