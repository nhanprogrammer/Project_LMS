using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class ClassTypeMapper : Profile
    {
        public ClassTypeMapper()
        {
            CreateMap<ClassType, ClassTypeResponse>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDelete.HasValue ? src.IsDelete.Value : false))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt ?? DateTime.MinValue));
            CreateMap<ClassType,UpdateClassTypeRequest>();
            CreateMap<ClassType, CreateClassTypeRequest>();
        }
    }
}