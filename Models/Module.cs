using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Module
    {
        public int Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Name { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
