using System.Collections;
using System.Text.Json.Serialization;
using FluentValidation;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.Data;

namespace Project_LMS.DTOs.Request
{
    public class TeacherRequest
    {
        public int? TeacherStatusId { get; set; }
        public int? SubjectGroupId { get; set; }
        public int? SubjectId { get; set; }
        public List<int?> TeacherSubjectIds { get; set; }
        public string? UserCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Image { get; set; }
        public bool? Gender { get; set; }
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? PlaceOfBirth { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? Card { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public string? CardIssuePlace { get; set; }
        public bool? UnionMember { get; set; }
        public DateTime? UnionJoinDate { get; set; }
        public bool? PartyMember { get; set; }
        public DateTime? PartyJoinDate { get; set; }
        public string? Address { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? Alias { get; set; }
        public string? National { get; set; }
        [JsonIgnore]
        public int? UserCreate { get; set; }
        [JsonIgnore]
        public int? UserUpdate { get; set; }
    }
    public class DeleteTeacherRequest 
    {
        public List<string> UserCodes { get; set; }
    }
    public class TeacherRequestValidator : AbstractValidator<TeacherRequest>
    {
        private readonly ApplicationDbContext _context;
        public TeacherRequestValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(tc => tc.TeacherStatusId).NotNull().WithMessage("TeacherStatusId không được để trống.")
                .GreaterThan(0).WithMessage(x => $"TeacherStatusId[{x.TeacherStatusId}] phải là số nguyên lớn hơn 0.")
                .Must(TeacherStatusExists).WithName(x => $"[{x.TeacherStatusId}] không tồn tại trong hệ thống.");

            RuleFor(tc => tc.SubjectGroupId).NotNull().WithMessage("SubjectGroupId không được để trống.")
                .GreaterThan(0).WithMessage(x => $"SubjectGroupId[{x.SubjectGroupId}] phải là số nguyên lớn hơn 0.")
                .Must(SubjectGroupExits).WithName(x => $"[{x.SubjectGroupId}] không tồn tại trong hệ thống.");

            RuleFor(tc => tc.SubjectId).NotNull().WithMessage("SubjectId không được để trống.")
                .GreaterThan(0).WithMessage(x => $"SubjectId[{x.SubjectId}] phải là số nguyên lớn hơn 0.")
                .Must(x => SubjectExits(x)).WithName(x => $"[{x.SubjectId}] không tồn tại trong hệ thống.");

            RuleFor(tc => tc.TeacherSubjectIds)
                .NotNull().WithMessage("TeacherSubjectIds không được để trống.")
                .NotEmpty().WithMessage("TeacherSubjectIds phải chứa ít nhất một phần tử.")
                .ForEach(idRule =>
                {
                    idRule.GreaterThan(0).WithMessage(id => $"TeacherSubjectId [{id}] phải là số nguyên lớn hơn 0.");
                    idRule.Must(SubjectExits).WithMessage(id => $"[{id}] không tồn tại trong hệ thống");
                });


            RuleFor(tc => tc.UserCode)
                .NotEmpty().WithMessage("UserCode không được để trống.");


            RuleFor(tc => tc.FullName)
                .NotNull().WithMessage("FullName không được để trống.");

            RuleFor(e => e.Email)
                .NotNull().WithMessage("Email không được để trống.")
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không đúng định dạng.");


            RuleFor(tc => tc.StartDate)
                .NotNull().WithMessage("StartDate không được để trống.");

            RuleFor(tc => tc.Gender)
                .NotNull().WithMessage("Gender không được để trống.");

            RuleFor(tc => tc.Ethnicity)
                .NotNull().WithMessage("Ethnicity không được để trống.");

            RuleFor(tc => tc.Religion)
                .NotNull().WithMessage("Religion không được để trống.");

            RuleFor(tc => tc.PlaceOfBirth)
                .NotNull().WithMessage("PlaceOfBirth không được để trống.");

            RuleFor(tc => tc.BirthDate)
                .NotNull().WithMessage("BirthDate không được để trống.");

            RuleFor(tc => tc.Phone)
                .NotNull().WithMessage("Phone không được để trống.");

            RuleFor(tc => tc.Card)
                .NotNull().WithMessage("Card không được để trống.");

            RuleFor(tc => tc.CardIssueDate)
                .NotNull().WithMessage("CardIssueDate không được để trống.");

            RuleFor(tc => tc.CardIssuePlace)
                .NotNull().WithMessage("CardIssuePlace không được để trống.");

            RuleFor(tc => tc.UnionMember)
                .NotNull().WithMessage("UnionMember không được để trống.");

            RuleFor(tc => tc.UnionJoinDate)
                .NotNull().WithMessage("UnionJoinDate không được để trống.");

            RuleFor(tc => tc.PartyMember)
                .NotNull().WithMessage("PartyMember không được để trống.");

            RuleFor(tc => tc.PartyJoinDate)
                .NotNull().WithMessage("PartyJoinDate không được để trống.");

            RuleFor(tc => tc.Address)
                .NotNull().WithMessage("Address không được để trống.");

            RuleFor(tc => tc.ProvinceId)
                .NotNull().WithMessage("ProvinceId không được để trống.");

            RuleFor(tc => tc.DistrictId)
                .NotNull().WithMessage("DistrictId không được để trống.");

            RuleFor(tc => tc.WardId)
                .NotNull().WithMessage("WardId không được để trống.");

            RuleFor(tc => tc.Alias)
                .NotNull().WithMessage("Alias không được để trống.");

            RuleFor(tc => tc.National)
                .NotNull().WithMessage("National không được để trống.");

        }
        private bool TeacherStatusExists(int? teacherStatusId)
        {

            return _context.TeacherStatuses.Any(ts => ts.Id == teacherStatusId && ts.IsDelete ==false);
        }
        private bool SubjectGroupExits(int? subjectGroupId)
        {

            return _context.SubjectGroups.Any(sg => sg.Id == subjectGroupId && sg.IsDelete == false);
        }
        private bool SubjectExits(int? subjectId)
        {
            return _context.Subjects.Any(s => s.Id == subjectId && s.IsDelete == false);
        }



    }
}
