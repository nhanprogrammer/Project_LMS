using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class AssignmentDetail
    {
        public int Id { get; set; }
        public int? AssignmentId { get; set; }
        public int? AnswerId { get; set; }
        public bool? IsCorrect { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Answer? Answer { get; set; }
        public virtual Assignment? Assignment { get; set; }
    }
}
