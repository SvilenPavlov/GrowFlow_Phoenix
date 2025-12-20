namespace GrowFlow_Phoenix.Infrastructure.Phoenix.Services
{
    using AutoMapper;
    using GrowFlow_Phoenix.Data;
    using GrowFlow_Phoenix.DTOs.Phoenix.Employee;
    using GrowFlow_Phoenix.Infrastructure.Leviathan.Services;
    using GrowFlow_Phoenix.Models;
    using GrowFlow_Phoenix.Models.Phoenix;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class EmployeeService
    {
        private readonly PhoenixDbContext _db;
        private readonly LeviathanClient _leviathan;
        private readonly IMapper _mapper;
        private readonly string _leviathanProviderName;

        public EmployeeService(PhoenixDbContext db, IConfiguration configuration, LeviathanClient leviathan, IMapper mapper)
        {
            _db = db;
            _leviathan = leviathan;
            _mapper = mapper;
            _leviathanProviderName = configuration.GetValue<string>("LeviathanApi:ProviderName");
        }

        public async Task<EmployeeResponseDTO> CreateAsync(EmployeeCreateDTO dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            employee.Id = Guid.NewGuid();

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            try
            {
                employee.LeviathanId = await _leviathan.CreateEmployeeAsync(employee);
                employee.IsSynced = true;
                employee.LastSyncedAt = DateTime.UtcNow;

                var employeeExternalId = new EmployeeExternalId
                {
                    EmployeeId = employee.Id,
                    Provider = _leviathanProviderName,
                    ExternalId = employee.LeviathanId == null ? string.Empty : employee.LeviathanId.ToString()
                };
                _db.EmployeeExternalIds.Add(employeeExternalId);
            }
            catch 
            {
                employee.IsSynced = false; // Means just leviathan sync is unsucessful, not Phoenix save
            }

            var responseDTO = _mapper.Map<EmployeeResponseDTO>(employee);
            responseDTO.ExternallySyncedEntries = new Dictionary<string, bool>
            {
                { _leviathanProviderName, employee.IsSynced }
            };

            await _db.SaveChangesAsync();
            return responseDTO;
        }

        public async Task<List<Employee>> GetAllAsync() =>
            await _db.Employees.AsNoTracking().ToListAsync();

        public async Task SyncFromLeviathanAsync(CancellationToken stopToken)
        {
            var leviathanEmployees = await _leviathan.GetEmployeesAsync(stopToken);

            foreach (var leviathanEmployee in leviathanEmployees)
            {
                if (_db.Employees.Any(e => e.LeviathanId == e.LeviathanId))
                {
                    var localEmployee = _db.Employees.FirstOrDefault(localEmp => localEmp.LeviathanId == leviathanEmployee.LeviathanId);
                    _mapper.Map(leviathanEmployee, localEmployee); // we consider Leviathan a source of truth so no validation is performed on whether it contains empty/null values, although it might be a good idea to do so
                }
            }

            await _db.SaveChangesAsync(stopToken);
        }
    }
}
