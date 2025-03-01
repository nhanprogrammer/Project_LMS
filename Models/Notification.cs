using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Notification
    {
        public Notification()
        {
            NotificationsReceivers = new HashSet<NotificationsReceiver>();
        }

        public int Id { get; set; }
        public int? SenderId { get; set; }
        public int? ClassId { get; set; }
        public int? TestExamId { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public bool? IsType { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Class? Class { get; set; }
        public virtual User? Sender { get; set; }
        public virtual TestExam? TestExam { get; set; }
        public virtual ICollection<NotificationsReceiver> NotificationsReceivers { get; set; }
    }
}
