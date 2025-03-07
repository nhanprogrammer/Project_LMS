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
        public int SchoolId { get; set; }
        public string BranchName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Manager { get; set; } = null!;
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual School School { get; set; } = null!;
        public virtual ICollection<SchoolTransfer> SchoolTransfers { get; set; }
    }
}
