using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class DependentMapper : Profile
    {
        public DependentMapper() {
            CreateMap<DependentRequest, Dependent>();
        }
    }
}
