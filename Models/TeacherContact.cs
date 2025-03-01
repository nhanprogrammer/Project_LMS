using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TeacherContact
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public string? ContactName { get; set; }
        public string? Relation { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual User? Teacher { get; set; }
    }
}
