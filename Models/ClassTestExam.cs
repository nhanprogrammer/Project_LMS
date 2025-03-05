using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ClassTestExam
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int TestExamId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual TestExam TestExam { get; set; } = null!;
    }
}
