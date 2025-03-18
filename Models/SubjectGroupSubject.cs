using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SubjectGroupSubject
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectGroupId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Subject? Subject { get; set; }
        public virtual SubjectGroup? SubjectGroup { get; set; }
    }
}
