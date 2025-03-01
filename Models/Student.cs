using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Student
    {
        public Student()
        {
            Assignments = new HashSet<Assignment>();
            ClassStudents = new HashSet<ClassStudent>();
            ClassStudentsOnlines = new HashSet<ClassStudentsOnline>();
            SchoolTransfers = new HashSet<SchoolTransfer>();
            StudentGuardians = new HashSet<StudentGuardian>();
            StudentParents = new HashSet<StudentParent>();
        }

        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public DateOnly BirthDate { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string StudentCode { get; set; } = null!;
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public DateOnly? AdmissionDate { get; set; }
        public string? StudyMode { get; set; }
        public string Status { get; set; } = null!;
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual AcademicYear AcademicYear { get; set; } = null!;
        public virtual User? User { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<ClassStudent> ClassStudents { get; set; }
        public virtual ICollection<ClassStudentsOnline> ClassStudentsOnlines { get; set; }
        public virtual ICollection<SchoolTransfer> SchoolTransfers { get; set; }
        public virtual ICollection<StudentGuardian> StudentGuardians { get; set; }
        public virtual ICollection<StudentParent> StudentParents { get; set; }
    }
}
