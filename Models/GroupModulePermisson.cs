using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class GroupModulePermisson
    {
        public GroupModulePermisson()
        {
            Users = new HashSet<User>();
            ModulePermissions = new HashSet<ModulePermission>();
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ModulePermission> ModulePermissions { get; set; }
    }
}
