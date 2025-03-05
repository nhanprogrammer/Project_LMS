using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class AcademicYear
    {
        public AcademicYear()
        {
            Classes = new HashSet<Class>();
            Semesters = new HashSet<Semester>();
        }

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsInherit { get; set; }
        public int? AcademicParent { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Semester> Semesters { get; set; }
    }
}
