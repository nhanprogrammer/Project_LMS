using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class DisciplineMapper : Profile
    {
        public DisciplineMapper()
        {
            CreateMap<Discipline, DisciplineResponse>()
                .ForMember(dest => dest.DisciplineContent, opt => opt.MapFrom(src => src.DisciplineContent))  // Map DisciplineContent
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt));

            CreateMap<Discipline,CreateDisciplineRequest >();
            CreateMap<Discipline, UpdateDisciplineRequest>();
        }
    }
}