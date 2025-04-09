using System.Text.Json.Serialization;
using FluentValidation;
using Project_LMS.Data;

namespace Project_LMS.DTOs.Request
{
    public class DisciplineRequest
    {
        public string? UserCode { get; set; }
        public int? SemesterId { get; set; }
        public string? FileName { get; set; }
        public string? DisciplineContent { get; set; }
        [JsonIgnore]
        public DateTime? CreateAt { get; set; }
        [JsonIgnore]
        public DateTime? UpdateAt { get; set; }
        [JsonIgnore]
        public int? UserCreate { get; set; }
        [JsonIgnore]
        public int? UserUpdate { get; set; }
    }
    public class UpdateDisciplineRequest : DisciplineRequest {
        public int id { get; set; }
    }
    public class DisciplineRequestValidator : AbstractValidator<DisciplineRequest>
    {
        private readonly ApplicationDbContext _context;
        public DisciplineRequestValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(x => x.UserCode).NotNull().WithMessage("UserCode không được để trống")
                .Must(UserExists).WithMessage("UserCode không tồn tại trong hệ thống.")
                               .Must(StatusExists).WithMessage("Học viên không thuộc trạng thái đang đi học không được kỷ luật."); ;
            RuleFor(x => x.SemesterId).NotNull().WithMessage("SemesterId không được để trống")
                .GreaterThan(0).WithMessage("Mã học viên phải lớn hơn 0.")
                .Must(SemesterExists).WithMessage("SemesterId không tồn tại trong hệ thống.");
            RuleFor(x => x.DisciplineContent).NotNull().WithMessage("DisciplineContent không được để trống");
        }
        private bool UserExists(string? userCode)
        {
            if (userCode == null) return false; // Kiểm tra ID hợp lệ

            var user = _context.Users
                .FirstOrDefault(u => u.UserCode == userCode && u.IsDelete == false);

            return user != null;
        }
        private bool StatusExists(string? userCode)
        {
            if (userCode == null) return false; // Kiểm tra ID hợp lệ

            var user = _context.Users
                .FirstOrDefault(u => u.UserCode == userCode && u.IsDelete == false && u.StudentStatusId == 1);

            return user != null;
        }
        private bool SemesterExists(int? semesterId)
        {
            if (semesterId <= 0) return false;

            var user = _context.Semesters
                .FirstOrDefault(u => u.Id == semesterId && u.IsDelete == false);

            return user != null;
        }
    }
}
