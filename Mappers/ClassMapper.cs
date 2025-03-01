using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class ClassMapper : Profile
    {
        public ClassMapper()
        {
            CreateMap<Class, ClassResponse>()
                .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.AcademicYearId))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.ClassType, opt => opt.MapFrom(src => src.ClassType.Name))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt ?? DateTime.MinValue))
                .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => src.IsDelete ?? false))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.UserCreate.ToString()))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UserUpdate.ToString()));
            CreateMap<CreateClassRequest, Class>();
            CreateMap<UpdateClassRequest, Class>();
        }
    }
}