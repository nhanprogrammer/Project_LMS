using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TeacherClassSubject
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubjectsId { get; set; }
        public bool? IsPrimary { get; set; }

        public virtual Subject Subjects { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
