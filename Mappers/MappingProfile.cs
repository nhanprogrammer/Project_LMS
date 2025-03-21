using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
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
            //.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.HoldDate, opt => opt.MapFrom(src => src.HoldDate))
            .ForMember(dest => dest.HoldDuration, opt => opt.MapFrom(src => src.HoldDuration))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName));
        //.ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
        //.ForMember(dest => dest.UserCreate, opt => opt.MapFrom(src => src.UserCreate));

        CreateMap<UpdateAcademicHoldRequest, AcademicHold>()
            //.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.HoldDate, opt => opt.MapFrom(src => src.HoldDate))
            .ForMember(dest => dest.HoldDuration, opt => opt.MapFrom(src => src.HoldDuration))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName));
            //.ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.UpdateAt))
            //.ForMember(dest => dest.UserCreate, opt => opt.MapFrom(src => src.UserUpdate));
        CreateMap<AcademicHold, AcademicHoldResponse>();



        CreateMap<AcademicYear, AcademicYearResponse>();
        CreateMap<CreateAcademicYearRequest, AcademicYear>()
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeHelper.Now)); // Chuyển về UTC+7
        CreateMap<UpdateAcademicYearRequest, AcademicYear>()
            .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeHelper.Now)); // Chuyển về UTC+7

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
            //.ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => src.IsDelete ?? false))
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

        CreateMap<ClassStudentOnline, ClassStudentOnlineResponse>();

        CreateMap<ClassStudentOnline, CreateClassStudentOnlineRequest>();
        CreateMap<ClassStudentOnline, UpdateClassStudentOnlineRequest>();

        CreateMap<ClassType, ClassTypeResponse>()
            // .ForMember(dest => dest.IsDeleted,
            //     opt => opt.MapFrom(src => src.IsDelete.HasValue ? src.IsDelete.Value : false))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt ?? DateTime.MinValue));
        CreateMap<ClassType, UpdateClassTypeRequest>();
        CreateMap<ClassType, CreateClassTypeRequest>();

        CreateMap<CreateDepartmentRequest, Department>()
            .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.departmentCode))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.userId))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.createAt ?? DateTime.Now));

        CreateMap<UpdateDepartmentRequest, Department>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => src.isDelete))
            .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.updateAt ?? DateTime.Now))
            .ForMember(dest => dest.UserUpdate, opt => opt.MapFrom(src => src.userUpdate));

        CreateMap<Department, DepartmentResponse>()
            .ForMember(dest => dest.DepartmentID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.DepartmentCode))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "N/A"));


        CreateMap<Discipline, DisciplineResponse>()
            .ForMember(dest => dest.DisciplineContent,
                opt => opt.MapFrom(src => src.DisciplineContent)) // Map DisciplineContent
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt));

        CreateMap<Discipline, CreateDisciplineRequest>();
        CreateMap<Discipline, UpdateDisciplineRequest>();

        CreateMap<Favourite, FavouriteResponse>()
            .ForMember(dest => dest.QuestionsAnswerId,
                opt => opt.MapFrom(src => src.QuestionsAnswerId)) // Map QuestionsAnswerId
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Map UserId
            .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.TopicId)); // Map TopicId
        CreateMap<Favourite, CreateFavouriteRequest>();
        CreateMap<Favourite, UpdateFavouriteRequest>();

        CreateMap<Lesson, LessonResponse>()
            .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassId)) // Map ClassId
            //    .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))  // Map TeacherId
            .ForMember(dest => dest.ClassLessonCode,
                opt => opt.MapFrom(src => src.ClassLessonCode)) // Map ClassLessonCode
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)) // Map Description
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.Topic)) // Map Topic
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration)) // Map Duration
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            //    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))  // Map Password
            .ForMember(dest => dest.IsResearchable, opt => opt.MapFrom(src => src.IsResearchable)) // Map IsResearchable
            .ForMember(dest => dest.IsAutoStart, opt => opt.MapFrom(src => src.IsAutoStart)) // Map IsAutoStart
            .ForMember(dest => dest.IsSave, opt => opt.MapFrom(src => src.IsSave)); // Map IsSave3
        
        CreateMap<Lesson, CreateLessonRequest>();
        CreateMap<CreateLessonRequest, Lesson>()
           .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeHelper.Now)); // Chuyển về UTC+7
        CreateMap<Lesson, UpdateLessonRequest>();
        CreateMap<UpdateLessonRequest, Lesson>()
            .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeHelper.Now)); // Chuyển về UTC+7

        CreateMap<Module, ModuleResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        //   .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        CreateMap<Module, CreateModuleRequest>();
        CreateMap<Module, UpdateModuleRequest>();

        CreateMap<TestExamType, TestExamTypeResponse>();
        CreateMap<TestExamTypeRequest, TestExamType>();

        // Mapper Create Test_exam for Test_Exam
        CreateMap<CreateTestExamRequest, TestExam>()
            // Map cột SemestersId, SubjectId, TestExamTypeId, ...
            .ForMember(dest => dest.SemestersId, opt => opt.MapFrom(src => src.semestersId))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.subjectId))
            .ForMember(dest => dest.TestExamTypeId, opt => opt.MapFrom(src => src.testExamTypeId))

            // Topic, IsExam, Form, Description, ScheduleStatusId, DepartmentId
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.topic))
            .ForMember(dest => dest.IsExam, opt => opt.MapFrom(src => src.isExam))
            .ForMember(dest => dest.Form, opt => opt.MapFrom(src => src.form))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
            .ForMember(dest => dest.ScheduleStatusId, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.departmentId))

            // DurationInMinutes -> Duration (TimeOnly?)
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src =>
                src.durationInMinutes.HasValue
                    ? TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(src.durationInMinutes.Value))
                    : (TimeOnly?)null
            ))

            // ExamDate -> StartDate (hoặc EndDate, tùy ý)
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.examDate))
            // Nếu bạn muốn EndDate giống StartDate:
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.examDate))
            // hoặc .ForMember(dest => dest.EndDate, opt => opt.Ignore())

            // Không map ClassId và UserId (vì quan hệ N-N, ta xử lý riêng)
            .ForMember(dest => dest.ClassId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            // Tự động set CreateAt, UpdateAt, IsDelete
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => false))

            // Bỏ qua các navigation collection (ClassTestExams, Examiners, ...)
            .ForMember(dest => dest.ClassTestExams, opt => opt.Ignore())
            .ForMember(dest => dest.Examiners, opt => opt.Ignore())
            .ForMember(dest => dest.Assignments, opt => opt.Ignore())
            .ForMember(dest => dest.Questions, opt => opt.Ignore());

        // Mapper Update Test_exam for Test_Exam
        CreateMap<UpdateTestExamRequest, TestExam>()
            // Map các thuộc tính trực tiếp
            .ForMember(dest => dest.SemestersId, opt => opt.MapFrom(src => src.semestersId))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.subjectId))
            .ForMember(dest => dest.TestExamTypeId, opt => opt.MapFrom(src => src.testExamTypeId))
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.topic))
            .ForMember(dest => dest.IsExam, opt => opt.MapFrom(src => src.isExam))
            .ForMember(dest => dest.Form, opt => opt.MapFrom(src => src.form))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
            .ForMember(dest => dest.ScheduleStatusId, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.departmentId))

            // Chuyển đổi DurationInMinutes -> Duration (TimeOnly?)
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src =>
                src.durationInMinutes.HasValue
                    ? TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(src.durationInMinutes.Value))
                    : (TimeOnly?)null
            ))

            // Map ExamDate sang StartDate và EndDate
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.examDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.examDate))

            // Không map các thuộc tính của mối quan hệ N-N (xử lý riêng)
            .ForMember(dest => dest.ClassId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())

            // Trong update, thông thường không update CreateAt; chỉ cập nhật UpdateAt
            .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.Now))

            // Thiết lập IsDelete (nếu logic nghiệp vụ yêu cầu)
            .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => false))

            // Bỏ qua các navigation collection (xử lý riêng nếu cần)
            .ForMember(dest => dest.ClassTestExams, opt => opt.Ignore())
            .ForMember(dest => dest.Examiners, opt => opt.Ignore())
            .ForMember(dest => dest.Assignments, opt => opt.Ignore())
            .ForMember(dest => dest.Questions, opt => opt.Ignore());


        CreateMap<TestExam, TestExamResponse>()
            .ForMember(dest => dest.Semester,
                opt => opt.MapFrom(src => src.Semesters.Name)) // Ánh xạ thuộc tính Semesters.Name
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Class.Department.Name)) // Ánh xạ Class.Department.Name
            .ForMember(dest => dest.SubjectName,
                opt => opt.MapFrom(src => src.Subject.SubjectName)) // Ánh xạ Subject.SubjectName
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.TestExamType.PointTypeName)) // Ánh xạ TestExamType.PointTypeName
            .ForMember(dest => dest.StatusExam,
                opt => opt.MapFrom(src => src.ExamScheduleStatus.Names)) // Ánh xạ ExamScheduleStatus.Names
            .ForMember(dest => dest.Examiner, opt => opt.MapFrom(src => src.User.FullName));

        CreateMap<Subject, SubjectResponse>();
        CreateMap<SubjectRequest, Subject>();
        CreateMap<SchoolTransfer, SchoolTransferResponse>();

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
                opt => opt.MapFrom(src =>
                    src.CreateAt.HasValue
                        ? TimeZoneInfo.ConvertTimeFromUtc(src.CreateAt.Value, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")) // Múi giờ GMT+7
                        : (DateTime?)null))

            .ForMember(dest => dest.UserCreate, opt => opt.MapFrom(src => src.UserCreate))
            .ForMember(dest => dest.SubjectGroupSubjects,
                opt => opt.MapFrom(src => MapSubjectGroupSubjects(src.SubjectIds)));


        CreateMap<TestExam, TestExamResponse>()
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semesters.Name))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TestExamType.PointTypeName))
            .ForMember(dest => dest.StatusExam, opt => opt.MapFrom(src => src.ExamScheduleStatus.Names))
            .ForMember(dest => dest.Examiner,
                opt => opt.MapFrom(src => string.Join(", ", src.Examiners.Select(e => e.User.FullName))));
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