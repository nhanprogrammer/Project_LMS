using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class ClassOnlineMapper : Profile
    {
        public ClassOnlineMapper()
        {
            CreateMap<ClassOnline, ClassOnlineResponse>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
                .ForMember(dest => dest.ClassCode, opt => opt.MapFrom(src => src.ClassCode))
                .ForMember(dest => dest.ClassTitle, opt => opt.MapFrom(src => src.ClassTitle))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.ClassDescription, opt => opt.MapFrom(src => src.ClassDescription))
                .ForMember(dest => dest.MaxStudents, opt => opt.MapFrom(src => src.MaxStudents))
                .ForMember(dest => dest.CurrentStudents, opt => opt.MapFrom(src => src.CurrentStudents))
                .ForMember(dest => dest.ClassStatus, opt => opt.MapFrom(src => src.ClassStatus))
                .ForMember(dest => dest.ClassLink, opt => opt.MapFrom(src => src.ClassLink));
            CreateMap<CreateClassOnlineRequest, ClassOnline>();
            CreateMap<UpdateClassOnlineRequest, ClassOnline>();
        }
    }
}