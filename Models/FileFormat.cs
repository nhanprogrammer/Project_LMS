using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class FileFormat
    {
        public int Id { get; set; }
        public int? TestExamId { get; set; }
        public bool? IsDoc { get; set; }
        public bool? IsPowerpoint { get; set; }
        public bool? IsXxls { get; set; }
        public bool? IsJpeg { get; set; }
        public bool? Is10 { get; set; }
        public bool? Is20 { get; set; }
        public bool? Is30 { get; set; }
        public bool? Is40 { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
