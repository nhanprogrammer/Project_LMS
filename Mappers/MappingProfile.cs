using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // CreateMap<AcademicHold, AcademicHoldResponse>()
        //         .ForMember(ah => ah.StudentCode, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().StudentCode))
        //         .ForMember(ah => ah.StudentName, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().FullName))
        //         .ForMember(ah => ah.BirthDate, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().BirthDate))
        //         .ForMember(ah => ah.Gender, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().Gender))
        //         .ForMember(ah => ah.ClassCode, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().ClassStudents.FirstOrDefault().Class.ClassCode))
        //         .ForMember(ah => ah.SemesterName, opt => opt.MapFrom(src => src.User.Students.FirstOrDefault().AcademicYear.Semesters.FirstOrDefault().Name));

        CreateMap<CreateAcademicHoldRequest, AcademicHold>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.HoldDate, opt => opt.MapFrom(src => src.HoldDate))
            .ForMember(dest => dest.HoldDuration, opt => opt.MapFrom(src => src.HoldDuration))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.UserCreate, opt => opt.MapFrom(src => src.UserCreate));

        CreateMap<UpdateAcademicHoldRequest, AcademicHold>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StudentId))
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


        CreateMap<Class, ClassResponse>()
               .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.AcademicYearId))
               .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
               .ForMember(dest => dest.ClassType, opt => opt.MapFrom(src => src.ClassType.Name))
               .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt ?? DateTime.MinValue))
               .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => src.IsDelete ?? false))
               .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.UserCreate.ToString()))
               .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UserUpdate.ToString()));
        CreateMap<CreateClassRequest, Class>();
        CreateMap<UpdateClassRequest, Class>();

        CreateMap<ClassOnline, ClassOnlineResponse>()
        //    .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src..FullName))
           .ForMember(dest => dest.ClassCode, opt => opt.MapFrom(src => src.ClassCode))
           .ForMember(dest => dest.ClassTitle, opt => opt.MapFrom(src => src.ClassTitle))
           .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
           .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
           .ForMember(dest => dest.ClassDescription, opt => opt.MapFrom(src => src.ClassDescription))
           .ForMember(dest => dest.MaxStudents, opt => opt.MapFrom(src => src.MaxStudents))
           .ForMember(dest => dest.CurrentStudents, opt => opt.MapFrom(src => src.CurrentStudents))
           .ForMember(dest => dest.ClassStatus, opt => opt.MapFrom(src => src.ClassStatus))
           .ForMember(dest => dest.ClassLink, opt => opt.MapFrom(src => src.ClassLink));
        CreateMap<CreateClassOnlineRequest, ClassOnline>();
        CreateMap<UpdateClassOnlineRequest, ClassOnline>();

        CreateMap<ClassStudentsOnline, ClassStudentOnlineResponse>();
        //    .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName));

        CreateMap<ClassStudentsOnline, CreateClassStudentOnlineRequest>();
        CreateMap<ClassStudentsOnline, UpdateClassStudentOnlineRequest>();

        CreateMap<ClassType, ClassTypeResponse>()
              .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDelete.HasValue ? src.IsDelete.Value : false))
              .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt ?? DateTime.MinValue));
        CreateMap<ClassType, UpdateClassTypeRequest>();
        CreateMap<ClassType, CreateClassTypeRequest>();

        CreateMap<Department, DepartmentResponse>()
           .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.Id))  // Map Id to DepartmentCode
           .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Name));  // Map Name to DepartmentName
        CreateMap<Department, CreateDepartmentRequest>();
        CreateMap<Department, UpdateDepartmentRequest>();

        CreateMap<Discipline, DisciplineResponse>()
            .ForMember(dest => dest.DisciplineContent, opt => opt.MapFrom(src => src.DisciplineContent))  // Map DisciplineContent
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt));

        CreateMap<Discipline, CreateDisciplineRequest>();
        CreateMap<Discipline, UpdateDisciplineRequest>();

        CreateMap<Favourite, FavouriteResponse>()
           .ForMember(dest => dest.QuestionsAnswerId, opt => opt.MapFrom(src => src.QuestionsAnswerId)) // Map QuestionsAnswerId
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Map UserId
           .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.TopicId)); // Map TopicId
        CreateMap<Favourite, CreateFavouriteRequest>();
        CreateMap<Favourite, UpdateFavouriteRequest>();

        CreateMap<Lesson, LessonResponse>()
           .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassId))  // Map ClassId
        //    .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))  // Map TeacherId
           .ForMember(dest => dest.ClassLessonCode, opt => opt.MapFrom(src => src.ClassLessonCode))  // Map ClassLessonCode
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))  // Map Description
           .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.Topic))  // Map Topic
           .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))  // Map Duration
           .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))  // Map StartDate
           .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))  // Map EndDate
        //    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))  // Map Password
           .ForMember(dest => dest.IsResearchable, opt => opt.MapFrom(src => src.IsResearchable))  // Map IsResearchable
           .ForMember(dest => dest.IsAutoStart, opt => opt.MapFrom(src => src.IsAutoStart))  // Map IsAutoStart
           .ForMember(dest => dest.IsSave, opt => opt.MapFrom(src => src.IsSave));  // Map IsSave3
        CreateMap<Lesson, CreateLessonRequest>();
        CreateMap<Lesson, UpdateLessonRequest>();

        CreateMap<Module, ModuleResponse>()
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        //   .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        CreateMap<Module, CreateModuleRequest>();
        CreateMap<Module, UpdateModuleRequest>();

        CreateMap<TestExamType, TestExamTypeResponse>();
        CreateMap<TestExamTypeRequest, TestExamType>();

        CreateMap<Subject, SubjectResponse>();
        CreateMap<SubjectRequest, Subject>();

        CreateMap<SubjectsGroup, SubjectsGroupResponse>();
        CreateMap<SubjectsGroupRequest, SubjectsGroup>();

        CreateMap<SubjectType, SubjectTypeResponse>();
        CreateMap<SubjectTypeRequest, SubjectType>();
    }
}