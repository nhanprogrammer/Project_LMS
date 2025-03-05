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
        public string PointTypeName { get; set; } = null!;
        public int? Coefficient { get; set; }
        public int? MinimunEntriesSem1 { get; set; }
        public int? MinimunEntriesSem2 { get; set; }

        public virtual ICollection<TestExam> TestExams { get; set; }
    }
}
