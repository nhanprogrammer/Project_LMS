using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ChatMessage
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public string? MessageContent { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsQuestion { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
