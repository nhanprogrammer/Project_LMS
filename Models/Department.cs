using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Department
    {
        public Department()
        {
            Classes = new HashSet<Class>();
            SubjectsGroups = new HashSet<SubjectsGroup>();
            TestExams = new HashSet<TestExam>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<SubjectsGroup> SubjectsGroups { get; set; }
        public virtual ICollection<TestExam> TestExams { get; set; }
    }
}
