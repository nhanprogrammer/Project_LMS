using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Class
    {
        public Class()
        {
            ChatMessages = new HashSet<ChatMessage>();
            ClassStudentsOnlines = new HashSet<ClassStudentsOnline>();
            ClassTestExams = new HashSet<ClassTestExam>();
            Lessons = new HashSet<Lesson>();
            TeachingAssignments = new HashSet<TeachingAssignment>();
            TestExams = new HashSet<TestExam>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int ClassTypeId { get; set; }
        public int AcademicYearId { get; set; }
        public int UserId { get; set; }
        public string? Description { get; set; }
        public string ClassLink { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ClassCode { get; set; } = null!;
        public string PasswordClass { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StatusClass { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual AcademicYear AcademicYear { get; set; } = null!;
        public virtual ClassType ClassType { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ClassStudentsOnline> ClassStudentsOnlines { get; set; }
        public virtual ICollection<ClassTestExam> ClassTestExams { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; }
        public virtual ICollection<TestExam> TestExams { get; set; }
    }
}
