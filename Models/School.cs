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
        public string SchoolCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Principal { get; set; } = null!;
        public string PrincipalPhone { get; set; } = null!;
        public string? Image { get; set; }
        public string Email { get; set; } = null!;
        public string Website { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string SchoolType { get; set; } = null!;
        public string EducationModel { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime EstablishmentDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<SchoolBranch> SchoolBranches { get; set; }
    }
}
