﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models
{
    public partial class SchoolTransfer
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime? TransferDate { get; set; }
        public BitArray? Status { get; set; }

        [Column("transfer_from")]
        public string? TransferFrom { get; set; } // Trường chuyển đến

        [Column("transfer_to")]
        public string? TransferTo { get; set; } // Trường chuyển đi

        [Column("province_id")]
        public string? ProvinceId { get; set; }

        [Column("district_id")]
        public string? DistrictId { get; set; }

        public string? Semester { get; set; }

        [Column("filename")]
        public string? FileName { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        public string? Reason { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual User? User { get; set; }
    }
}