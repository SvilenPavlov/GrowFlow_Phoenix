using AutoMapper;
using GrowFlow_Phoenix.DTOs.Leviathan.Employee;
using GrowFlow_Phoenix.Models.Leviathan;
using GrowFlow_Phoenix.Models.Phoenix;

namespace GrowFlow_Phoenix
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<GrowFlow_Phoenix.DTOs.Phoenix.Employee.EmployeeCreateDTO, Employee>().ReverseMap();
            CreateMap<GrowFlow_Phoenix.DTOs.Leviathan.Employee.EmployeeCreateDTO, Employee>().ReverseMap();
            CreateMap<Employee,GrowFlow_Phoenix.DTOs.Phoenix.Employee.EmployeeResponseDTO>().ReverseMap();
            CreateMap<LeviathanEmployeeCache, EmployeeResponseDTO>().ReverseMap();
        }
    }
}
