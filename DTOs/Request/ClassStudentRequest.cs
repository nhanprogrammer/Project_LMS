using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Project_LMS.Data;

namespace Project_LMS.DTOs.Request
{
    public class ClassStudentRequest
    {
        [Required(ErrorMessage = "ClassId không được bỏ trống.")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "UserId không được bỏ trống.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Reason không được bỏ trống.")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "FileName không được bỏ trống.")]
        public string FileName { get; set; }
        [Required(ErrorMessage = "ChangeDate không được bỏ trống.")]
        public DateTime ChangeDate { get; set; }

        [JsonIgnore]
        public int? UserCreate { get; set; }

        [JsonIgnore]
        public int? UserUpdate { get; set; }
    }

    public class ClassStudentRequestValidator : AbstractValidator<ClassStudentRequest>
    {
        private readonly ApplicationDbContext _context;

        public ClassStudentRequestValidator(ApplicationDbContext context)
        {
            _context = context;

            // Custom rule to check class existence and academic year match
            RuleFor(cs => cs).Custom((cs, context) =>
            {
                if (!ClassExists(cs.ClassId))
                {
                    context.AddFailure("ClassId", "Lớp học không tồn tại trong hệ thống.");
                }
                if (!ClassExists(cs.ClassId) && !StudentExists(cs.UserId))
                {
                    if (!CheckClass(cs.ClassId, cs.UserId))
                    {
                        context.AddFailure("ClassId", "Lớp học không thuộc niên khóa hiện tại.");
                    }
                }
            });

            RuleFor(cs => cs.UserId)
                .Must(StudentExists)
                .WithMessage("UserId không tồn tại trong hệ thống.");
            RuleFor(cs => cs.ChangeDate)
    .GreaterThanOrEqualTo(DateTime.Now)
    .WithMessage("Ngày thay đổi không được nhỏ hơn ngày hiện tại.");
        }

        private bool ClassExists(int classId)
        {
            return _context.Classes.Any(c => c.Id == classId && c.IsDelete == false);
        }

        private bool StudentExists(int studentId)
        {
            return _context.Users.Any(s => s.Id == studentId && s.IsDelete == false);
        }

        private bool CheckClass(int classId, int studentId)
        {
            var classStudent = _context.ClassStudents.FirstOrDefault(cs => cs.UserId == studentId && cs.IsActive == true);
            //if (classStudent?.Class == null) return false;

            return _context.Classes.Any(c => c.Id == classId && c.AcademicYearId == classStudent.Class.AcademicYearId);
        }
    }
}
