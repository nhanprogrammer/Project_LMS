using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SubmissionFile
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string? FileName { get; set; }
    }
}
