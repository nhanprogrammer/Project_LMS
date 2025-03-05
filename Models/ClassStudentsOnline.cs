using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ClassStudentsOnline
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public bool? IsMuted { get; set; }
        public bool? IsCamera { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? JoinTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public DateTime? RemoveAt { get; set; }
        public DateTime? AddedAt { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
