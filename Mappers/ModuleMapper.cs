using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class ModuleMapper : Profile
    {
        public ModuleMapper()
        {

            CreateMap<Module, ModuleResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))  
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
            CreateMap<Module, CreateModuleRequest>();
            CreateMap<Module, UpdateModuleRequest>();
        }
    }
}