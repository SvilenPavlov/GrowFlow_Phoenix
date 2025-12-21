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
        private readonly IMapper _mapper;
        private readonly string _leviathanProviderName;
        private readonly LeviathanSyncManager _leviathanSyncManager;

        public EmployeeService(PhoenixDbContext db, IConfiguration configuration, IMapper mapper, LeviathanSyncManager leviathanSyncManager)
        {
            _db = db;
            _mapper = mapper;
            _leviathanProviderName = configuration.GetValue<string>("LeviathanApi:ProviderName")!;
            _leviathanSyncManager = leviathanSyncManager;
        }

        public async Task<EmployeeResponseDTO> CreateAsync(EmployeeCreateDTO dto, CancellationToken stopToken)
        {
            var employee = _mapper.Map<Employee>(dto);
            employee.Id = Guid.NewGuid();

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            /* The below operation is Leviathan specific but can be abstracted by implementing an inrerface for sync managers and calling each one in separate methods for exapmle. 
             * This would include injecting the interface in the constructor instead of the Leviathan-specfic one and defining a manager dictionary somewhere to easily auto-pick corresponding manager for each required sync service.
             * Did not imppement this to avoid scope creep.
            */ 
            var externalSyncServices = new Dictionary<string, bool>();
            try
            {
                var isLeviathanSyncSuccessful  = await _leviathanSyncManager.CreateEmployee(employee, stopToken);
                externalSyncServices.Add(_leviathanProviderName, isLeviathanSyncSuccessful); 
            }
            catch
            {
                 // Only Leviathan sync is unsucessful, Phoenix save and syncs to other API's unaffected
            }

            var responseDTO = _mapper.Map<EmployeeResponseDTO>(employee);

            responseDTO.ExternallySyncedEntries = externalSyncServices;

            //await _db.SaveChangesAsync(stopToken);
            return responseDTO;
        }

        public async Task<List<EmployeeResponseDTO>> GetAllAsync(CancellationToken stopToken)
        {
            var employees = await _db.Employees.AsNoTracking().ToListAsync(stopToken);
            return _mapper.Map<List<EmployeeResponseDTO>>(employees);
        }

    }
}
