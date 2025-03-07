﻿using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class SubjectsGroup
    {
        public SubjectsGroup()
        {
            Subjects = new HashSet<Subject>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public string Name { get; set; } = null!;
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Subject Subject { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
