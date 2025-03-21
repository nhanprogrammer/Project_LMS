﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models
{
    public partial class ClassStudent
    {
        public int Id { get; set; }
        public int? ClassId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
        [Column("is_active")] // Đảm bảo tên cột đúng với PostgreSQL
        public bool? IsActive { get; set; }
        public virtual Class? Class { get; set; }
        public virtual User? User { get; set; }
    }
}
