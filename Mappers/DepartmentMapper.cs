using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class DepartmentMapper : Profile
    {
        public DepartmentMapper()
        {
    
            CreateMap<Department, DepartmentResponse>()
                .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.Id))  // Map Id to DepartmentCode
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Name));  // Map Name to DepartmentName
            CreateMap<Department, CreateDepartmentRequest>();
            CreateMap<Department, UpdateDepartmentRequest>();
        }
    }
}