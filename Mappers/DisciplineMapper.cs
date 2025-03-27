using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class DisciplineMapper : Profile
    {
        public DisciplineMapper() {
            CreateMap<DisciplineRequest, Discipline>();
        }
    }
}
