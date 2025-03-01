using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TestExamType
    {
        public TestExamType()
        {
            TestExams = new HashSet<TestExam>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<TestExam> TestExams { get; set; }
    }
}
