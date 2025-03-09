using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TeacherClassSubject
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? SubjectsId { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Subject? Subjects { get; set; }
        public virtual User? User { get; set; }
    }
}
