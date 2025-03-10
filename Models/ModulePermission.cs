using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ModulePermission
    {
        public ModulePermission()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public int? ModuleId { get; set; }
        public int? GroupRoleId { get; set; }
        public bool? IsView { get; set; }
        public bool? IsInsert { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? EnterScore { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
