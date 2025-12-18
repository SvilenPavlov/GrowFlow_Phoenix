namespace GrowFlow_Phoenix.Services
{
    using AutoMapper;
    using GrowFlow_Phoenix.Data;
    using GrowFlow_Phoenix.DTOs.Phoenix.Employee;
    using GrowFlow_Phoenix.Models;
    using Microsoft.EntityFrameworkCore;

    public class EmployeeService
    {
        private readonly PhoenixDbContext _db;
        private readonly LeviathanClient _leviathan;
        private readonly IMapper _mapper;
        private readonly string leviathanProviderName = "Leviathan";

        public EmployeeService(PhoenixDbContext db, LeviathanClient leviathan, IMapper mapper)
        {
            _db = db;
            _leviathan = leviathan;
            _mapper = mapper;
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
                //TODO add logic to check if this worked and handle it here
                employee.IsSynced = true;
                employee.LastSyncedAt = DateTime.UtcNow;

                //Create related db entry:
                var employeeExternalId = new EmployeeExternalId
                {
                    EmployeeId = employee.Id,
                    Provider = leviathanProviderName,
                    ExternalId = employee.LeviathanId
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
                { leviathanProviderName, employee.IsSynced }
            };

            await _db.SaveChangesAsync();
            return responseDTO;
        }

        public async Task<List<Employee>> GetAllAsync() =>
            await _db.Employees.AsNoTracking().ToListAsync();

        //public async Task SyncFromLeviathanAsync()
        //{
        //    var employees = await _leviathan.GetEmployeesAsync();

        //    foreach (var e in employees)
        //    {
        //        if (!_db.Employees.Any(e => e.LeviathanId == e.LeviathanId))
        //        {
        //            _db.Employees.Add(e);
        //        }
        //    }

        //    await _db.SaveChangesAsync();
        //}
    }
}
