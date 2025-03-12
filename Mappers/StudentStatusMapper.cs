using AutoMapper;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class StudentStatusMapper : Profile
    {
        public StudentStatusMapper()
        {
            CreateMap<StudentStatus, StudentStatusResponse>();
        }
    }
}
