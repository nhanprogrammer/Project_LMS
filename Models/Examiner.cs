using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Examiner
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestExamId { get; set; }

        public virtual TestExam TestExam { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
