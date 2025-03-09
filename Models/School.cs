using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class School
    {
        public School()
        {
            SchoolBranches = new HashSet<SchoolBranch>();
        }

        public int Id { get; set; }
        public string? SchoolCode { get; set; }
        public string? Name { get; set; }
        public string? Principal { get; set; }
        public string? PrincipalPhone { get; set; }
        public string? Image { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Province { get; set; }
        public bool? Thcs { get; set; }
        public bool? Thpt { get; set; }
        public string? EducationModel { get; set; }
        public string? Phone { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<SchoolBranch> SchoolBranches { get; set; }
    }
}
