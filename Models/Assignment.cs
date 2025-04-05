using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Assignment
    {
        public Assignment()
        {
            AssignmentDetails = new HashSet<AssignmentDetail>();
        }

        public int Id { get; set; }
        public int? TestExamId { get; set; }
        public int? UserId { get; set; }
        public int? StatusAssignmentId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public Double? TotalScore { get; set; }
        public string? SubmissionFile { get; set; }
        public bool? IsSubmit { get; set; }
        public string? Comment { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual StatusAssignment? StatusAssignment { get; set; }
        public virtual TestExam? TestExam { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; }
    }
}