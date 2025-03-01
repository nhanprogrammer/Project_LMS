using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers;

public class ClassStudentMapper : Profile
{
    protected ClassStudentMapper()
    {
        CreateMap<ClassStudent, ClassStudentOnlineResponse>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName));
        CreateMap<CreateClassStudentRequest, ClassStudent>();
        CreateMap<ClassStudent, UpdateClassStudentRequest>();
    }

   
}