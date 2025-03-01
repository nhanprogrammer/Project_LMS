using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class District
    {
        public District()
        {
            SchoolTransfers = new HashSet<SchoolTransfer>();
            Wards = new HashSet<Ward>();
        }

        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int ProvinceCode { get; set; }
        public string Name { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string CodeName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string FullNameEnd { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Province ProvinceCodeNavigation { get; set; } = null!;
        public virtual ICollection<SchoolTransfer> SchoolTransfers { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }
    }
}
