using System;
using System.Collections.Generic;

namespace LMS.Models
{
    public partial class GroupModulePermisson
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
