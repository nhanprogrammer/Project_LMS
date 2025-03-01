using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
            AssignmentDetails = new HashSet<AssignmentDetail>();
        }

        public int Id { get; set; }
        public int TestExamId { get; set; }
        public string Question1 { get; set; } = null!;
        public int Mark { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual TestExam TestExam { get; set; } = null!;
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; }
    }
}
