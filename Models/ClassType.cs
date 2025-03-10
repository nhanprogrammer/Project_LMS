﻿using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class ClassType
    {
        public ClassType()
        {
            Classes = new HashSet<Class>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? Status { get; set; }
        public string? Note { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}
