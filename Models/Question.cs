using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        public int Id { get; set; }
        public int? TestExamId { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public double? Mark { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual TestExam? TestExam { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
