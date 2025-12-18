using AutoMapper;
using GrowFlow_Phoenix.Data;

namespace GrowFlow_Phoenix
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<GrowFlow_Phoenix.DTOs.Phoenix.Employee.EmployeeCreateDTO, Employee>().ReverseMap();
            CreateMap<GrowFlow_Phoenix.DTOs.Leviathan.Employee.EmployeeCreateDTO, Employee>().ReverseMap();
        }
    }
}
