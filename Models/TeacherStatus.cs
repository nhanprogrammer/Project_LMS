using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TeacherStatus
    {
        public TeacherStatus()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? StatusName { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
