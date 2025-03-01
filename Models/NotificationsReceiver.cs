using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class NotificationsReceiver
    {
        public int Id { get; set; }
        public int? NotificationId { get; set; }
        public int? ReceiverId { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Notification? Notification { get; set; }
        public virtual User? Receiver { get; set; }
    }
}
