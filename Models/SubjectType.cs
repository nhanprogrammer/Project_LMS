using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SubjectType
    {
        public SubjectType()
        {
            Subjects = new HashSet<Subject>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
