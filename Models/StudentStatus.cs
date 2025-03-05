using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class StudentStatus
    {
        public StudentStatus()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? StatusName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
