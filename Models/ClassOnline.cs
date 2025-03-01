using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ClassOnline
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public string ClassCode { get; set; } = null!;
        public string ClassTitle { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClassDescription { get; set; }
        public int? MaxStudents { get; set; }
        public int? CurrentStudents { get; set; }
        public bool? ClassStatus { get; set; }
        public string? ClassLink { get; set; }
        public string? ClassPassword { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Teacher Teacher { get; set; } = null!;
    }
}
