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
        public string? PointTypeName { get; set; }
        public int? Coefficient { get; set; }
        public int? MinimunEntriesSem1 { get; set; }
        public int? MinimunEntriesSem2 { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<TestExam> TestExams { get; set; }
    }
}
