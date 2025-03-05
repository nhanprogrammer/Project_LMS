using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SystemSetting
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool? CaptchaEnabled { get; set; }
        public string? CurrentTheme { get; set; }
        public string? Language { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
