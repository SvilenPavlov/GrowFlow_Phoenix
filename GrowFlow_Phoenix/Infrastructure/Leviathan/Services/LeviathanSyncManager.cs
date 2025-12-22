using AutoMapper;
using GrowFlow_Phoenix.Data;
using GrowFlow_Phoenix.Models.Leviathan;
using GrowFlow_Phoenix.Models.Phoenix;
using GrowFlow_Phoenix.Models.Utility.IUtility;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace GrowFlow_Phoenix.Infrastructure.Leviathan.Services
{
    public class LeviathanSyncManager
    {
        private readonly PhoenixDbContext _db;
        private readonly LeviathanClient _client;
        private readonly ILogger<LeviathanSyncService> _logger;
        private readonly IMapper _mapper;
        private readonly string _leviathanProviderName;

        public LeviathanSyncManager(PhoenixDbContext db, ILogger<LeviathanSyncService> logger, IConfiguration configuration, IMapper mapper, LeviathanClient client)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _leviathanProviderName = configuration.GetValue<string>("LeviathanApi:ProviderName");
            _client = client;
        }

        public async Task RetryUnsuccesfullySyncedEmployees(CancellationToken stopToken)
        {
            var unsyncedEmployees = await _db.Employees
                .Where(e => e.ExternalIds.Any(x => x.Provider == _leviathanProviderName) == false)
                .ToListAsync(stopToken);

            foreach (var employee in unsyncedEmployees)
            {
                var result = CreateEmployee(employee, stopToken); // TODO log results in caller method
            }
        }

        public async Task DownloadSnapshot(CancellationToken stopToken)
        {
            try
            {
                var freshLeviathanDtos = await _client.GetEmployeesAsync(stopToken);

                // Returns employee id and corresponding Leviathan external id for all employees that have one .e.g. Phoenix employees already recorded in Leviathan
                var employeeExternalIdsMap = _db.Employees
              .Where(e => e.ExternalIds.Any(x => x.Provider == _leviathanProviderName))
              .Select(e => new
              {
                  EmployeeId = e.Id,
                  ExternalId = e.ExternalIds
                               .Where(x => x.Provider == _leviathanProviderName)
                               .Select(x => x.ExternalId)
                               .FirstOrDefault()
              })
              .ToDictionary(x => x.EmployeeId, x => x.ExternalId);

                foreach (var employeeExternalIdEntry in employeeExternalIdsMap)
                {
                    var freshLeviathanDto = freshLeviathanDtos.FirstOrDefault(x => x.LeviathanId.ToString() == employeeExternalIdEntry.Value);
                    var freshCacheEntry = _mapper.Map<LeviathanEmployeeCache>(freshLeviathanDto);
                    var existingCacheEntry = await _db.LeviathanEmployeeCacheEntries.FirstOrDefaultAsync(c => c.LeviathanId == freshCacheEntry.LeviathanId);

                    if (existingCacheEntry == null)
                    {
                        _db.LeviathanEmployeeCacheEntries.Add(existingCacheEntry!);
                    }

                    _db.Entry(existingCacheEntry!).CurrentValues.SetValues(freshCacheEntry);
                }

                await _db.SaveChangesAsync(stopToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Leviathan failure"); // TODO: Log results of this method in logger
            }
        }

        public async Task SynceEmployeesFromSnapshot(CancellationToken stopToken)
        {
            var cacheEntries = await _db.LeviathanEmployeeCacheEntries.ToListAsync();
            // Filters only phoenix employees that have a corresponding record in Leviathan already
            var phoenixEmployees = await _db.Employees
                .Include(e => e.ExternalIds)
                .Where(e => e.ExternalIds
                    .Any(x => x.Provider == _leviathanProviderName && cacheEntries
                        .Select(c => c.LeviathanId.ToString())
                        .Contains(x.ExternalId)))
                .ToListAsync();

            try
            {
                for (int i = 0; i < phoenixEmployees.Count(); i++)
                {
                    var phoenixEmployeeMatch = phoenixEmployees[i];
                    var phoenixLeviathanExternalId = phoenixEmployeeMatch.ExternalIds.FirstOrDefault(x => x.Provider == _leviathanProviderName);
                    var cacheEntry = cacheEntries.FirstOrDefault(x => x.LeviathanId.ToString() == phoenixLeviathanExternalId!.ExternalId);

                    // This always returns true since we've already fitlered above but it felt cool implementing a custom comparer so I am keeping it.
                    if ((phoenixEmployeeMatch as IEmployeeComparable)?.IsEquivalent(cacheEntry!) == true) //Force not-null as we've already filtered phoenixEmployees to only those that have a corresponding cache entry
                    {
                        _mapper.Map(cacheEntry, phoenixEmployeeMatch); // We consider Leviathan a source of truth so no validation is performed on whether it contains empty/null values, although it might be a good idea to do so
                        _db.Update(phoenixEmployeeMatch!);
                    }
                }
                await _db.SaveChangesAsync(stopToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during mapping Leviathan employees to Phoenix employees");
            }
        }

        public async Task<bool> CreateEmployee(Employee employee, CancellationToken stopToken)
        {
            bool isLeviathanSyncSuccessful = false;

            var employeeLeviathanGuid = await _client.CreateEmployeeAsync(employee);
            if (!string.IsNullOrEmpty(employeeLeviathanGuid.ToString()) && !string.IsNullOrWhiteSpace(employeeLeviathanGuid.ToString()))
            {
                isLeviathanSyncSuccessful = true;
                var externalId = new EmployeeExternalId
                {
                    EmployeeId = employee.Id,
                    Provider = _leviathanProviderName,
                    ExternalId = employeeLeviathanGuid.ToString()
                };
                employee.ExternalIds.Add(externalId);
                await _db.SaveChangesAsync(stopToken);
            }
            return isLeviathanSyncSuccessful;
        }
    }
}


