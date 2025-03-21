using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models
{
    public partial class Examiner
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? TestExamId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
        //Foreign key
        [Column("class_id")]
        public int? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; } = null!;
        public virtual TestExam? TestExam { get; set; }
        public virtual User? User { get; set; }
    }
}