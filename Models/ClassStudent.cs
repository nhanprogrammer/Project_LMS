using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ClassStudent
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
