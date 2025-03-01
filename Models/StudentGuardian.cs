using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class StudentGuardian
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string GuardianName { get; set; } = null!;
        public int? GuardianBirthYear { get; set; }
        public string? GuardianOccupation { get; set; }
        public string? GuardianPhone { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Student Student { get; set; } = null!;
    }
}
