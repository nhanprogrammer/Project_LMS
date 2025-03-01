using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class LessonMapper : Profile
    {
        public LessonMapper()
        {
            // Define the mapping between Lesson and LessonResponse
            CreateMap<Lesson, LessonResponse>()
                .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassId))  // Map ClassId
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))  // Map TeacherId
                .ForMember(dest => dest.ClassLessonCode, opt => opt.MapFrom(src => src.ClassLessonCode))  // Map ClassLessonCode
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))  // Map Description
                .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.Topic))  // Map Topic
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))  // Map Duration
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))  // Map StartDate
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))  // Map EndDate
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))  // Map Password
                .ForMember(dest => dest.IsResearchable, opt => opt.MapFrom(src => src.IsResearchable))  // Map IsResearchable
                .ForMember(dest => dest.IsAutoStart, opt => opt.MapFrom(src => src.IsAutoStart))  // Map IsAutoStart
                .ForMember(dest => dest.IsSave, opt => opt.MapFrom(src => src.IsSave));  // Map IsSave3
            CreateMap<Lesson, CreateLessonRequest>();
            CreateMap<Lesson, UpdateLessonRequest>();

        }
    }
}