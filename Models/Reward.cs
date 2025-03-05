using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Reward
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SemesterId { get; set; }
        public int RewardCode { get; set; }
        public string RewardName { get; set; } = null!;
        public DateTime RewardDate { get; set; }
        public string? RewardContent { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Semester Semester { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
