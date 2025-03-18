using AutoMapper;
using Project_LMS.Models;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections;
namespace Project_LMS.Mappers
{
    public class StudentMapper : Profile
    {
        public StudentMapper()
        {
            CreateMap<StudentRequest,User>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => new BitArray(new bool[] { (bool)src.Gender })));

            CreateMap<UpdateStudentRequest, User>()
    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => new BitArray(new bool[] { (bool)src.Gender })));

            CreateMap<User, StudentResponse>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender != null && src.Gender.Length > 0 ? src.Gender[0] : false));
        }
    }
}
