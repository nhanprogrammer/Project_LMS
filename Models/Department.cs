using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_LMS.Models
{
    public partial class Department
    {
        public Department()
        {
            Classes = new HashSet<Class>();
        }

        public int Id { get; set; }
        public string DepartmentCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        
        //Foreign key
        [Column("user_id")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        public virtual ICollection<Class> Classes { get; set; }
        public ICollection<TestExam> TestExams { get; set; }

    }
}