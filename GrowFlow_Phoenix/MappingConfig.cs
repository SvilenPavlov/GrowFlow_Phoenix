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
            CreateMap<DTOs.Phoenix.Employee.EmployeeCreateDTO, Employee>().ReverseMap();
            CreateMap<LeviathanEmployeeCache, EmployeeResponseDTO>().ReverseMap();
            CreateMap<Employee, LeviathanEmployeeCache>();
            CreateMap<LeviathanEmployeeCache, Employee>()
                .ForMember(dest=>dest.Id, opt=>opt.Ignore());
            CreateMap<EmployeeCreateDTO, Employee>();
            CreateMap<Employee, EmployeeCreateDTO>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Role) ? "Placeholder Leviathan Role - Phoenix entry lacks role property" : src.Role));
            /* The magic string above is left here on purpose, to highlight the difference in the required model properties between the two APIs.
             * Leviathan does not accept empty Role values, but Role is not required in the Phoenix Employee model per the task description document.
            */

        }
    }
}
