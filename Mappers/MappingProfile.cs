using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AcademicHold, AcademicHoldResponse>()
                .ForMember(ah => ah.StudentCode, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().StudentCode))
                .ForMember(ah => ah.StudentName, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().FullName))
                .ForMember(ah => ah.BirthDate, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().BirthDate))
                .ForMember(ah => ah.Gender, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().Gender))
                .ForMember(ah => ah.ClassCode, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().ClassStudents.FirstOrDefault().Class.ClassCode))
                .ForMember(ah => ah.SemesterName, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().AcademicYear.Semesters.FirstOrDefault().Name));

        CreateMap<CreateAcademicHoldRequest, AcademicHold>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.HoldDate, opt => opt.MapFrom(src => src.HoldDate))
            .ForMember(dest => dest.HoldDuration, opt => opt.MapFrom(src => src.HoldDuration))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.UserCreate, opt => opt.MapFrom(src => src.UserCreate));

        CreateMap<UpdateAcademicHoldRequest, AcademicHold>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.HoldDate, opt => opt.MapFrom(src => src.HoldDate))
            .ForMember(dest => dest.HoldDuration, opt => opt.MapFrom(src => src.HoldDuration))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.UserCreate, opt => opt.MapFrom(src => src.UserUpdate));


        CreateMap<AcademicYear, AcademicYearResponse>();
        CreateMap<CreateAcademicYearRequest, AcademicYear>();
        CreateMap<UpdateAcademicYearRequest, AcademicYear>();

        CreateMap<Answer, AnswerResponse>()
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer1));
        CreateMap<CreateAnswerRequest, Answer>()
            .ForMember(dest => dest.Answer1, opt => opt.MapFrom(src => src.Answer));
        CreateMap<UpdateAnswerRequest, Answer>()
            .ForMember(dest => dest.Answer1, opt => opt.MapFrom(src => src.Answer));

        CreateMap<Assignment, AssignmentsResponse>();
        CreateMap<CreateAssignmentRequest, Assignment>();
        CreateMap<UpdateAssignmentRequest, Assignment>();

        CreateMap<AssignmentDetail, AssignmentDetailResponse>();
        CreateMap<CreateAssignmentDetailRequest, AssignmentDetail>();
        CreateMap<UpdateAssignmentDetailRequest, AssignmentDetail>();

        CreateMap<ChatMessage, ChatMessageResponse>();
        CreateMap<CreateChatMessageRequest, ChatMessage>();
        CreateMap<UpdateChatMessageRequest, ChatMessage>();

        CreateMap<Role, RoleResponse>().ReverseMap();
        CreateMap<RoleRequest, Role>().ReverseMap();

        CreateMap<School, SchoolResponse>().ReverseMap();
        CreateMap<SchoolRequest, School>().ReverseMap();

        CreateMap<SchoolBranch, SchoolBranchResponse>().ReverseMap();
        CreateMap<SchoolBranchRequest, SchoolBranch>().ReverseMap();

        CreateMap<SchoolTransfer, SchoolTransferResponse>().ReverseMap();
        CreateMap<SchoolTransferRequest, SchoolTransfer>().ReverseMap();

        CreateMap<Semester, SemesterResponse>().ReverseMap();
        CreateMap<SemesterRequest, Semester>().ReverseMap();
    }
}