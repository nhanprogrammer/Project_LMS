using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Class
    {
        public Class()
        {
            ChatMessages = new HashSet<ChatMessage>();
            ClassStudents = new HashSet<ClassStudent>();
            ClassStudentsOnlines = new HashSet<ClassStudentsOnline>();
            Lessons = new HashSet<Lesson>();
            Notifications = new HashSet<Notification>();
            TeacherClassSubjects = new HashSet<TeacherClassSubject>();
            TeachingAssignments = new HashSet<TeachingAssignment>();
        }

        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int DepartmentId { get; set; }
        public int ClassTypeId { get; set; }
        public string? Description { get; set; }
        public string ClassCode { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual ClassType ClassType { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ClassStudent> ClassStudents { get; set; }
        public virtual ICollection<ClassStudentsOnline> ClassStudentsOnlines { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<TeacherClassSubject> TeacherClassSubjects { get; set; }
        public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; }
    }
}
