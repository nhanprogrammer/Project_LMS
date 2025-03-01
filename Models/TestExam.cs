using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TestExam
    {
        public TestExam()
        {
            Assignments = new HashSet<Assignment>();
            Notifications = new HashSet<Notification>();
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int TestExamTypeId { get; set; }
        public string? Topic { get; set; }
        public string? Form { get; set; }
        public TimeOnly? Duration { get; set; }
        public bool? Classify { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Attachment { get; set; }
        public string? SubmissionFormat { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual TestExamType TestExamType { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
