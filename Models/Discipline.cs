using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models
{
    public partial class Discipline
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? SemesterId { get; set; }
        //public int? DisciplineCode { get; set; }
        [Column("file_name")]
        public string? FileName { get; set; }
        public DateTime? DisciplineDate { get; set; }
        public string? DisciplineContent { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual Semester? Semester { get; set; }
        public virtual User? User { get; set; }
    }
}
