using System.Collections;
using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            // Map từ UserRequest (bool) sang User (BitArray)
            CreateMap<UserRequest, User>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => new BitArray(new bool[] { (bool)src.Gender })));

            // Map từ User (BitArray) sang UserResponse (bool)
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender != null && src.Gender.Length > 0 ? src.Gender[0] : false));
        }
    }
}
