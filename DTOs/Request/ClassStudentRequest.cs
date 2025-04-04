using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Project_LMS.Data;

namespace Project_LMS.DTOs.Request
{
    public class ClassStudentRequest
    {
        [Required(ErrorMessage ="ClassId không được bỏ trống.")]
        public int ClassId { get; set; }
        [Required(ErrorMessage = "UserId không được bỏ trống.")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Name không được bỏ trống.")]
        public string Reason { get; set; }
        [Required(ErrorMessage = "ClassId không được bỏ trống.")]
        public string FileName { get; set; }
        public DateTime ChangeDate { get; set; }

        [JsonIgnore]
        public int? UserCreate { get; set; }
        [JsonIgnore]
        public int? UserUpdate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
    public class ClassStudentRequestValiator : AbstractValidator<ClassStudentRequest>
    {
        private readonly ApplicationDbContext _context;

        public ClassStudentRequestValiator(ApplicationDbContext context)
        {
            _context = context;
            //RuleFor(cs=>cs.ClassId)
        }

        private bool ClassExists(int classId)
        {
            return _context.Classes.Any(c=>c.Id == classId && c.IsDelete == false);
        }   
        private bool StudentExists(int studentId)
        {
            return _context.Users.Any(s=>s.Id == studentId && s.IsDelete == false);
        }   
        private bool CheckClass(int classId, DateTime changeDate)
        {
            return _context.Classes.Any(c=>c.Id == classId && changeDate >= c.AcademicYear.StartDate && changeDate <= c.AcademicYear.EndDate );
        }

    }
}
