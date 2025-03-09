using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Examiner
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? TestExamId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual TestExam? TestExam { get; set; }
        public virtual User? User { get; set; }
    }
}
