using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class AcademicHold
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime? HoldDate { get; set; }
        public int? HoldDuration { get; set; }
        public string? Reason { get; set; }
        public string? FileName { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual User? User { get; set; }
    }
}
