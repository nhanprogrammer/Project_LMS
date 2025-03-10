using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SubmissionFile
    {
        public int Id { get; set; }
        public int? AssignmentId { get; set; }
        public string? FileName { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
