using FluentValidation;
using Project_LMS.Data;
using Project_LMS.Models;

namespace Project_LMS.DTOs.Request
{
    public class TranscriptRequest
    {
        public int DepartmentId { get; set; } //Khối
        public int StudentId { get; set; }
        public int SemesterId { get; set; }
    }
    public class TranscriptRequestValidator : AbstractValidator<TranscriptRequest>
    {
        private readonly ApplicationDbContext _context;

        public TranscriptRequestValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(t => t.DepartmentId).NotNull().WithMessage("DepartmentId không được để trống.")
                .GreaterThan(0).WithMessage("DepartmentId phải là số nguyên.")
                .Must(DepartmentExists).WithName("DepartmentId không tồn tại.");

            RuleFor(t => t.StudentId).NotNull().WithMessage("StudentId không được để trống.")
                .GreaterThan(0).WithMessage("StudentId phải là số nguyên.")
                .Must(StudentExists).WithName("StudentId không tồn tại.");

            RuleFor(t => t.SemesterId).NotNull().WithMessage("SemesterId không được để trống.")
                .GreaterThan(0).WithMessage("SemesterId phải là số nguyên.")
                .Must(SemesterExists).WithName("SemesterId không tồn tại.");
        }
        private bool DepartmentExists(int departmentId)
        {
            return _context.Departments.Any(d => d.Id == departmentId && d.IsDelete == false);
        }
        private bool StudentExists(int studentId)
        {
            return _context.Users.Any(st => st.Id == studentId && st.IsDelete == false);
        }
        private bool SemesterExists(int semesterId)
        {
            return _context.Semesters.Any(st => st.Id == semesterId && st.IsDelete == false);
        }
    }
    public class TranscriptTeacherRequest
    {
        public int ClassId { get; set; }
        public int SemesterId { get; set; }
        public int SubjectId { get; set; }
        public string? searchItem { get; set; }
    }
    public class TranscriptTeacherRequestValidator : AbstractValidator<TranscriptTeacherRequest>
    {
        private readonly ApplicationDbContext _context;

        public TranscriptTeacherRequestValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(t => t.ClassId).NotNull().WithMessage("ClassId không được để trống.")
                .GreaterThan(0).WithMessage("ClassId phải là số nguyên.")
                .Must(ClassExists).WithName("ClassId không tồn tại.");



            RuleFor(t => t.SemesterId).NotNull().WithMessage("SemesterId không được để trống.")
                .GreaterThan(0).WithMessage("SemesterId phải là số nguyên.")
                .Must(SemesterExists).WithName("SemesterId không tồn tại.");

            RuleFor(t => t.SubjectId).NotNull().WithMessage("SubjectId không được để trống.")
                .GreaterThan(0).WithMessage("SubjectId phải là số nguyên.")
                .Must(SubjectExists).WithName("SubjectId không tồn tại.");
        }
        private bool ClassExists(int classId)
        {
            return _context.Classes.Any(c => c.Id == classId && c.IsDelete == false);
        }
        private bool SubjectExists(int subjectId)
        {
            return _context.Subjects.Any(s => s.Id == subjectId && s.IsDelete == false);
        }
        private bool SemesterExists(int semesterId)
        {
            return _context.Semesters.Any(st => st.Id == semesterId && st.IsDelete == false);
        }
    }
}
