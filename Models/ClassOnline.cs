using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ClassOnline
    {
        public ClassOnline()
        {
            ChatMessages = new HashSet<ChatMessage>();
            ClassStudentOnlines = new HashSet<ClassStudentOnline>();
        }
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? ClassOnlineCode { get; set; }
        public string? ChatCode { get; set; }
        public int? LessonId { get; set; }
        public string? ClassTitle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClassDescription { get; set; }
        public int? MaxStudents { get; set; }
        public int? CurrentStudents { get; set; }
        public bool? ClassStatus { get; set; }
        public string? ClassLink { get; set; }
        public string? ClassPassword { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ClassStudentOnline> ClassStudentOnlines { get; set; }
        public virtual Lesson? Lesson { get; set; }
    }
}