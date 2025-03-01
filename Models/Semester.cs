using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Semester
    {
        public Semester()
        {
            Disciplines = new HashSet<Discipline>();
            Rewards = new HashSet<Reward>();
        }

        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual AcademicYear AcademicYear { get; set; } = null!;
        public virtual ICollection<Discipline> Disciplines { get; set; }
        public virtual ICollection<Reward> Rewards { get; set; }
    }
}
