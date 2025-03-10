using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SchoolBranch
    {
        public SchoolBranch()
        {
            SchoolTransfers = new HashSet<SchoolTransfer>();
        }

        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string? BranchName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Manager { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual School? School { get; set; }
        public virtual ICollection<SchoolTransfer> SchoolTransfers { get; set; }
    }
}
