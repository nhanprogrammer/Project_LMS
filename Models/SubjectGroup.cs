using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SubjectGroup
    {
        public SubjectGroup()
        {
            SubjectGroupSubjects = new HashSet<SubjectGroupSubject>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<SubjectGroupSubject> SubjectGroupSubjects { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
