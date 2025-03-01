using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SchoolTransfer
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? SchoolBranchesId { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? Semester { get; set; }
        public string? Reason { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual District? District { get; set; }
        public virtual Province? Province { get; set; }
        public virtual Student? Student { get; set; }
        public virtual Ward? Ward { get; set; }
    }
}
