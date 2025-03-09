using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers;

public class SubjectGroupMapper : Profile
{
    public SubjectGroupMapper()
    {
        CreateMap<SubjectGroup, SubjectGroupResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown"))
            .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => 
            src.SubjectGroupSubjects != null && src.SubjectGroupSubjects.Any()
                ? src.SubjectGroupSubjects
                    .Where(sgs => sgs.SubjectGroupId == src.Id) 
                    .Select(sgs => new SubjectInfo
                    {
                        Id = sgs.Id,
                        SubjectCode = sgs.Subject != null ? sgs.Subject.SubjectCode : "Unknown",
                        SubjectName = sgs.Subject != null ? sgs.Subject.SubjectName : "Unknown"
                    }).ToList()
                : new List<SubjectInfo>()
        ));


        CreateMap<CreateSubjectGroupRequest, SubjectGroup>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreateAt,
                opt => opt.MapFrom(src => src.CreateAt.HasValue ? src.CreateAt.Value.ToLocalTime() : (DateTime?)null))
            .ForMember(dest => dest.UserCreate, opt => opt.MapFrom(src => src.UserCreate))
            .ForMember(dest => dest.SubjectGroupSubjects,
                opt => opt.MapFrom(src => MapSubjectGroupSubjects(src.SubjectIds)));

           

    }
    
    private List<SubjectGroupSubject> MapSubjectGroupSubjects(List<int> subjectIds)
    {
        var subjectGroupSubjects = new List<SubjectGroupSubject>();
        foreach (var subjectId in subjectIds)
        {
            subjectGroupSubjects.Add(new SubjectGroupSubject { SubjectId = subjectId });
        }

        return subjectGroupSubjects;
    }
}