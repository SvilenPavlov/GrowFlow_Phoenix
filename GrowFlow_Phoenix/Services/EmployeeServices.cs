namespace GrowFlow_Phoenix.Services
{
    using GrowFlow_Phoenix.Data;
    using GrowFlow_Phoenix.DTOs;
    using Microsoft.EntityFrameworkCore;

    public class EmployeeService
    {
        private readonly PhoenixDbContext _db;
        private readonly LeviathanClient _leviathan;

        public EmployeeService(PhoenixDbContext db, LeviathanClient leviathan)
        {
            _db = db;
            _leviathan = leviathan;
        }

        public async Task<Employee> CreateAsync(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Telephone = dto.Telephone
            };

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            try
            {
                employee.LeviathanId = await _leviathan.CreateEmployeeAsync(employee);
                employee.IsSynced = true;
                employee.LastSyncedAt = DateTime.UtcNow;
            }
            catch
            {
                employee.IsSynced = false;
            }

            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<List<Employee>> GetAllAsync() =>
            await _db.Employees.AsNoTracking().ToListAsync();

        public async Task SyncFromLeviathanAsync()
        {
            var remoteEmployees = await _leviathan.GetEmployeesAsync();

            foreach (var remote in remoteEmployees)
            {
                if (!_db.Employees.Any(e => e.LeviathanId == remote.LeviathanId))
                {
                    _db.Employees.Add(remote);
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
