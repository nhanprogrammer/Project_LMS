using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TestExam
    {
        public TestExam()
        {
            Assignments = new HashSet<Assignment>();
            ClassTestExams = new HashSet<ClassTestExam>();
            Examiners = new HashSet<Examiner>();
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int TestExamTypeId { get; set; }
        public int SemestersId { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public string? Topic { get; set; }
        public string? Form { get; set; }
        public TimeOnly? Duration { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Attachment { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
        public virtual Semester Semesters { get; set; } = null!;
        public virtual TestExamType TestExamType { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<ClassTestExam> ClassTestExams { get; set; }
        public virtual ICollection<Examiner> Examiners { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
