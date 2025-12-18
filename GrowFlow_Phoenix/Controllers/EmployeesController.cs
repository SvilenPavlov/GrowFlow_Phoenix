using GrowFlow_Phoenix.DTOs.Phoenix.Employee;
using GrowFlow_Phoenix.Services;
using Microsoft.AspNetCore.Mvc;

namespace GrowFlow_Phoenix.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _service;

        public EmployeesController(EmployeeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _service.GetAllAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateDTO dto)
        {
            var employee = await _service.CreateAsync(dto);
            return Ok(employee);
        }
    }

}
