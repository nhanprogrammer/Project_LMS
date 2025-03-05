using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int? SenderId { get; set; }
        public int? UserId { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public bool? Type { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual User? Sender { get; set; }
        public virtual User? User { get; set; }
    }
}
