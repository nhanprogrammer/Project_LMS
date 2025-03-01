using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class StudentParent
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? FatherName { get; set; }
        public int? FatherBirthYear { get; set; }
        public string? FatherOccupation { get; set; }
        public string? FatherPhone { get; set; }
        public string? MotherName { get; set; }
        public int? MotherBirthYear { get; set; }
        public string? MotherOccupation { get; set; }
        public string? MotherPhone { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Student Student { get; set; } = null!;
    }
}
