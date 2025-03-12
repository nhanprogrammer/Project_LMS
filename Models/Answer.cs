using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Answer
    {
        public Answer()
        {
            AssignmentDetails = new HashSet<AssignmentDetail>();
        }

        public int Id { get; set; }
        public int? QuestionId { get; set; }
        public string? Answer1 { get; set; }
        public bool? IsCorrect { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Question? Question { get; set; }
        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; }
    }
}
