using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.Models;
using Project_LMS.DTOs.Response;
using System.Collections;

namespace Project_LMS.Mappers
{
    public class TeacherMapper : Profile
    {
        public TeacherMapper()
        {
            CreateMap<TeacherRequest, User>()
                         .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => new BitArray(new bool[] { (bool)src.Gender }))); ;
            CreateMap<User, TeacherResponse>()
                                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender != null && src.Gender.Length > 0 ? src.Gender[0] : false)); ;
        }
    }
}
