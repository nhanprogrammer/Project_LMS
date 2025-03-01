using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Subject
    {
        public Subject()
        {
            TeacherClassSubjects = new HashSet<TeacherClassSubject>();
            TeachingAssignments = new HashSet<TeachingAssignment>();
        }

        public int Id { get; set; }
        public int TypeSubjectId { get; set; }
        public int SubjectGroupId { get; set; }
        public bool? IsStatus { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual SubjectsGroup SubjectGroup { get; set; } = null!;
        public virtual SubjectType TypeSubject { get; set; } = null!;
        public virtual ICollection<TeacherClassSubject> TeacherClassSubjects { get; set; }
        public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; }
    }
}
