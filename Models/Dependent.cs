using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Dependent
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DependentCode { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
