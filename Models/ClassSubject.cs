using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ClassSubject
    {
        public int Id { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Class? Class { get; set; }
        public virtual Subject? Subject { get; set; }
    }
}
