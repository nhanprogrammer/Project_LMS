using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Teacher
    {
        public Teacher()
        {
            ClassOnlines = new HashSet<ClassOnline>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string TeacherCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; } = null!;
        public string? Ethnicity { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public int DepartmentsId { get; set; }
        public int TeachingSubjectId { get; set; }
        public DateOnly? StartDate { get; set; }
        public string? Position { get; set; }
        public string Status { get; set; } = null!;
        public string? IdCard { get; set; }
        public DateOnly? IdCardIssueDate { get; set; }
        public string? IdCardIssuePlace { get; set; }
        public bool? UnionMember { get; set; }
        public DateOnly? UnionJoinDate { get; set; }
        public bool? PartyMember { get; set; }
        public DateOnly? PartyJoinDate { get; set; }
        public string? PartyJoinPlace { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<ClassOnline> ClassOnlines { get; set; }
    }
}
